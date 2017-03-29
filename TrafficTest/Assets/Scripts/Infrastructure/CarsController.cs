using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CarsController
{
    public static CarsController Instance { get; private set; }
    private readonly Graph _graph = new Graph();
    private readonly Dictionary<Hex, IEnumerable<Hex>> _pathsArray = new Dictionary<Hex, IEnumerable<Hex>>();
    public readonly Dijkstras Dijkstras = new Dijkstras();

    public CarsController()
    {
        Instance = this;

        var refuelings = Object.FindObjectsOfType<RefuelingMark>().
            Select(mark => mark.transform.position.ToHex()).ToArray();
        InitPathFinding();

        foreach (var pos in Object.FindObjectsOfType<CarSpawner>().
                                Select(spawner => spawner.Position).
                                SelectMany(spawnerPos => Constants.RoadDraft.Select(hex => hex + spawnerPos)))
            foreach (var refueling in refuelings)
            {
                var path = Dijkstras.shortest_path(pos, refueling).ToArray();
                if (!path.Any())
                {
                    Object.Instantiate(Departure.Prefab, Map.Instance.Layout.HexToVector3(pos),
                        Quaternion.identity).transform.SetParent(RoadPool.Instance.transform);
                    continue;
                }
                if (!_graph.Contains(pos) || _graph.Nodes[pos].Neighbors.Any(edge => edge.Value > path.Length))
                {
                    _graph.AddDirectedEdge(pos, refueling, path.Length);
                    _pathsArray[pos] = path;
                }
            }
        MapManager.Instance.StartCoroutine(CarGeneration());
    }
    /// <summary>
    /// Initializes path search only on objects that implement road markings
    /// </summary>
    private void InitPathFinding()
    {
        var allRoadHexes = Map.Instance.HexUnitMap.Where(pair => pair.Value.OtherObjects.ContainsKey(HexUnitObj.RoadOrientation)).
            ToDictionary(pair => pair.Key, pair => pair.Value);
        //MapManager.Instance.Draw(allRoadHexes);
        foreach (var road in allRoadHexes)
        {
            var orientations = (IEnumerable<Orientation>)road.Value.OtherObjects[HexUnitObj.RoadOrientation];
            var road1 = road;
            Dijkstras.add_vertex(road.Key,
                orientations.
                Select(orientation => Hex.Neighbor(road1.Key, (int)orientation)).Where(neighbor => allRoadHexes.ContainsKey(neighbor)));
        }
    }
    /// <summary>
    /// Generates car with specified frequency
    /// </summary>
    /// <returns></returns>
    private IEnumerator CarGeneration()
    {
        for (;;)
        {
            var car = Object.Instantiate(Car.Prefab[Random.Range(0, Car.Prefab.Length)]);
            var keys = _pathsArray.Keys.ToArray();
            var index = Random.Range(0, keys.Length);
            var path = _pathsArray[keys[index]];
            var enumerable = path as Hex[] ?? path.ToArray();
            car.Position = enumerable.First();
            car.MoveTransform();
            car.SetOrientation((Orientation)Hex.Directions.IndexOf(enumerable[1] - enumerable[0]));
            car.Move(path);
            yield return new WaitForSeconds(1f + Constants.MaxCarFrequencyGen * PlayerPrefs.GetFloat(PrefKey.CarFrequency));
        }
    }
}