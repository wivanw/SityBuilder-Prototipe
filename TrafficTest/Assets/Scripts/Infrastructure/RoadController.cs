using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class RoadController
{
    public static RoadController Instance { get; private set; }
    public readonly List<HexEdge> Edges = new List<HexEdge>();
    public readonly Dijkstras Dijkstras = new Dijkstras();
    private readonly Map _map;
    private readonly List<Hex> _graph = new List<Hex>();
    private readonly List<Hex> _road = new List<Hex>();
    public static readonly Func<Hex, Hex> Unscaled = hex => hex * 2 + Hex.One;
    private static readonly Func<Hex, Hex> Scale = hex => new Hex((hex.X - 1) / 2, (hex.Y - 1) / 2);

    public RoadController()
    {
        Instance = this;
        _map = MapManager.Instance.Map;
        while (!BuildCarSpawner())
            ;
    }
    /// <summary>
    /// Create car spawner on the map
    /// </summary>
    /// <returns>Do car spawners paths have access to each other</returns>
    private bool BuildCarSpawner()
    {
        var carSpawners = CarSpawnerGen().ToList();
        InitPathFinding();
        if (_graph.GetRange(1, _graph.Count - 1).Any(pos => !Dijkstras.shortest_path(_graph.First(), pos).Any()))
        {
            _graph.Clear();
            _road.Clear();
            foreach (var spawners in carSpawners)
                Object.Destroy(spawners.gameObject);

            return false;
        }
        foreach (var spawner in carSpawners)
            _map.SetNewOwner(spawner);

        return true;
    }

    private IEnumerable<CarSpawner> CarSpawnerGen()
    {
        var obj = CarSpawner.Prefab;
        var carSpawners = new List<CarSpawner>(Constants.RoadEntryCount);
        while (_graph.Count < Constants.RoadEntryCount)//entry generation
        {
            Hex position;
            Hex unscaledPosition;
            var objSpawner = Object.Instantiate(obj).GetComponent<CarSpawner>();
            do
            {
                var edge = Random.Range(0, 4);
                var pos = Random.Range(0, Constants.WorldSize + 2) / 2;
                var arr = new[] { pos, Constants.WorldSize / 2 - 1, pos, 0 };
                position = new Hex(arr[edge], arr[(edge + 1) % 4]);
                unscaledPosition = Unscaled(position);
            }
            while (objSpawner.BottomDraft.Keys.Any(h => _map.IsOccupied(h + unscaledPosition)));
            objSpawner.Position = unscaledPosition;
            objSpawner.MoveTransform();
            _graph.Add(position);
            carSpawners.Add(objSpawner);
        }
        return carSpawners;
    }

    /// <summary>
    /// Build road
    /// </summary>
    public void BuildRoad()
    {
        BuildGraph();
        InitPathFinding();
        var edges = Edges.SelectMany(edge => Dijkstras.shortest_path(edge.A, edge.B,
            hex => _road.Contains(hex) ? 1 : (Rectifier(edge.A, edge.B, hex) ? 3 : 6)));
        _road.AddRange(_graph);
        foreach (var hex in edges.Where(hex => !_road.Contains(hex)))
            _road.Add(hex);

        foreach (var roadHex in _road)
            CreateRoadObj(roadHex, _road.Concat(Object.FindObjectsOfType<RoadAddMark>().
                Select(mark => Scale(mark.transform.position.ToHex()))));

        foreach (var mark in Object.FindObjectsOfType<RefuelingMark>())
            Map.Instance.HexUnitMap[mark.transform.position.ToHex()].AddTrigger(mark);
    }
    /// <summary>
    /// A check is whether a particular hex is on the straight line of one of the two current hexes
    /// Used to improve the styling of the path search path so that the roads are straight.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="finish"></param>
    /// <param name="third">Convertible hex</param>
    /// <returns></returns>
    public static bool Rectifier(Hex start, Hex finish, Hex third)
    {
        return start.X == third.X || finish.X == third.X || start.Y == third.Y || finish.Y == third.Y;
    }
    /// <summary>
    /// Launches the chain of identification of a part road 
    /// with a heavily loaded corresponding prefab
    /// </summary>
    /// <param name="position">Position in the world space</param>
    /// <param name="newRoad"></param>
    private static void CreateRoadObj(Hex position, IEnumerable<Hex> newRoad)
    {
        var neighbors = Hex.Neighbors(position).ToList();
        var orientations = neighbors.Where(newRoad.Contains).
            Select(neighbor => (Orientation)neighbors.IndexOf(neighbor)).ToArray();
        RoadDraftHelper.IdentifyRoadType(orientations, position);
    }
    /// <summary>
    /// Build planar graph of car spawners and points of fuel triggers
    /// </summary>
    private void BuildGraph()
    {
        _graph.AddRange(Object.FindObjectsOfType<RefuelingMark>().Select(refueling => Scale(refueling.transform.position.ToHex())));
        for (var i = 0; i < _graph.Count; i++)
            for (var j = i + 1; j < _graph.Count; j++)
            {
                var currentEdge = new HexEdge(_graph[i], _graph[j]);
                var isAdd = true;
                var removeEdges = new List<HexEdge>();
                var magnitude = Hex.Distance(currentEdge.A, currentEdge.B);
                foreach (var edge in Edges.Where(edge => Intersection(currentEdge, edge)))
                {
                    if (magnitude < Hex.Distance(edge.A, edge.B))
                        removeEdges.Add(edge);
                    else
                    {
                        isAdd = false;
                        break;
                    }
                }
                if (isAdd && !Edges.Contains(currentEdge))
                {
                    Edges.Add(currentEdge);
                    foreach (var edge in removeEdges)
                        Edges.Remove(edge);
                }
            }
    }
    /// <summary>
    /// Initializes a path search for possible road construction
    /// </summary>
    public void InitPathFinding()
    {
        var freeHexes = new List<Hex>(_graph);
        for (var i = 0; i < Constants.WorldSize / 2; i++)
            for (var j = 0; j < Constants.WorldSize / 2; j++)
            {
                var roadHex = new Hex(i, j);
                var hexPlane = Constants.RoadDraft.Select(hex => hex + Unscaled(roadHex));
                if (hexPlane.All(hex => !_map.IsOccupied(hex)))
                    freeHexes.Add(roadHex);
            }

        Dijkstras.Clear();
        foreach (var roadHex in freeHexes)
            Dijkstras.add_vertex(roadHex, Hex.Directions.Select(hex => hex + roadHex).Where(hex => freeHexes.Contains(hex)).ToArray());
    }
    /// <summary>
    /// Checks the possibility of laying a road from the object to any car spawners 
    /// in the given position of the object and with a given orientation in world space
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="position">Check object position</param>
    /// <param name="orientation">Check orientation in world space</param>
    /// <returns></returns>
    public bool PathFindingTest<T>(T obj, Hex position, Orientation orientation) where T : ParentOwner
    {
        position += ParentOwner.TurnDraft(obj.GetComponentInChildren<T>().Orientation,
            orientation,
            new Dictionary<Hex, Biom> {
            {
                obj.GetComponentInChildren<RefuelingMark>().transform.position.ToHex()
                , default(Biom)
            } }).First().Key;

        return !Map.Instance.IsOccupied(position) && Dijkstras.shortest_path(_graph.First(), Scale(position)).Any();
    }
    /// <summary>
    /// Check for the crossing of two edge of the graph
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Intersection(HexEdge a, HexEdge b)
    {
        return AreCrossing(a.A, a.B, b.A, b.B);
    }
    /// <summary>
    /// Cross product
    /// </summary>
    /// <param name="ax"></param>
    /// <param name="ay"></param>
    /// <param name="bx"></param>
    /// <param name="by"></param>
    /// <returns></returns>
    private static int VectorMult(int ax, int ay, int bx, int by)
    {
        return ax * by - bx * ay;
    }
    /// <summary>
    /// Check for the crossing of two edge of the graph
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <returns></returns>
    private static bool AreCrossing(Hex p1, Hex p2, Hex p3, Hex p4)//проверка пересечения
    {
        var v1 = VectorMult(p4.X - p3.X, p4.Y - p3.Y, p1.X - p3.X, p1.Y - p3.Y);
        var v2 = VectorMult(p4.X - p3.X, p4.Y - p3.Y, p2.X - p3.X, p2.Y - p3.Y);
        var v3 = VectorMult(p2.X - p1.X, p2.Y - p1.Y, p3.X - p1.X, p3.Y - p1.Y);
        var v4 = VectorMult(p2.X - p1.X, p2.Y - p1.Y, p4.X - p1.X, p4.Y - p1.Y);
        return v1 * v2 < 0 && v3 * v4 < 0;
    }

    //static Hex CulcCrossHex()
    //{
    //    LineEquation(a.A, a.B);
    //    var a1 = A;
    //    var b1 = B;
    //    var c1 = C;
    //    LineEquation(b.A, b.B);
    //    var a2 = A;
    //    var b2 = B;
    //    var c2 = C;
    //    var p = CrossingPoint(a1, b1, c1, a2, b2, c2);
    //}

    //построение уравнения прямой
    //static int A, B, C;//коэффициенты уравнения прямой вида: Ax+By+C=0
    //static void LineEquation(Hex p1, Hex p2)
    //{
    //    A = p2.Y - p1.Y;
    //    B = p1.X - p2.X;
    //    C = -p1.X * (p2.Y - p1.Y) + p1.Y * (p2.X - p1.X);
    //}
    ////поиск точки пересечения
    //static Hex CrossingPoint(int a1, int b1, int c1, int a2, int b2, int c2)
    //{
    //    var pt = new Hex();
    //    double d = a1 * b2 - b1 * a2;
    //    double dx = -c1 * b2 + b1 * c2;
    //    double dy = -a1 * c2 + c1 * a2;
    //    pt.X = (int)(dx / d);
    //    pt.Y = (int)(dy / d);
    //    return pt;
    //}
}

public enum RoadType { Simple, Intersection, Tshaped, Turn }