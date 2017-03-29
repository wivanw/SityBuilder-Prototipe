using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public Map Map;
    public RoadController RoadController;
    public BuildingController BuildingController;
    public CarsController CarsController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Map = new Map(Constants.WorldSize);
        TerrainManagers.Instance.InitTerrain(Map);
        RoadController = new RoadController();
        BuildingController = new BuildingController();
        RoadController.BuildRoad();
        CarsController = new CarsController();
#if UNITY_EDITOR
        //Draw(Map.HexUnitMap);
#endif
    }
#if UNITY_EDITOR
    private void Update()
    {
        foreach (var edge in RoadController.Edges)
            Debug.DrawLine(new Vector3(edge.A.X, 0.02f, edge.A.Y) * 2 + new Vector3(1, 0, 1), new Vector3(edge.B.X, 0.02f, edge.B.Y) * 2 + new Vector3(1, 0, 1), Color.green);
        Color[] colors = { Color.cyan, Color.black, Color.blue, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
        var i = 0;
        foreach (var nodes in CarsController.Dijkstras._vertices)
            foreach (var node in nodes.Value)
                Debug.DrawLine(Map.Layout.HexToVector3(nodes.Key), Map.Layout.HexToVector3(node), colors[i++ % colors.Length]);
    }

    public void Draw(Dictionary<Hex, HexUnit> map)
    {
        var hexPrefab = Resources.Load<GameObject>("Prefabs/Hex");
        //                        Plain,     Forest,      Road,       ParentOwner,    Occupied
        var colors = new[] { Color.gray, Color.black, Color.blue, Color.white, Color.black };
        foreach (var pair in map)
        {
            var hex = Instantiate(hexPrefab, new Vector3(pair.Key.X, 0, pair.Key.Y), Quaternion.identity);
            hex.GetComponentInChildren<Renderer>().material.color = colors[(int)pair.Value.Type];
            hex.transform.SetParent(transform);
        }
    }
#endif

    public bool TryMove<T>(T owner, Hex position) where T : IHexOwner, IHexTrigger<T>, IMovables
    {
        if (!Map.HexUnitMap[position].Trigger(owner))
            return false;

        try
        {
            ChangeHexOwner(owner, position);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Application.Quit();
            throw new ApplicationException();
        }
    }

    private void ChangeHexOwner<T>(T owner, Hex hex) where T : IHexOwner, IHexTrigger<T>, IMovables
    {
        foreach (var draftHex in owner.BottomDraft.Select(draftHex => draftHex.Key))
        {
            if (Map.HexUnitMap.ContainsKey(owner.Position + draftHex))
                Map.HexUnitMap[owner.Position + draftHex].RemoveTriggers(owner);
            if (Map.HexUnitMap.ContainsKey(hex + draftHex))
                Map.HexUnitMap[hex + draftHex].AddTrigger(owner);
            owner.SetPosition(hex);
        }
    }

    public void RemoveTrigger<T>(T owner) where T : IHexOwner, IHexTrigger<T>, IMovables
    {
        foreach (var hexDraft in owner.BottomDraft)
            if (Map.HexUnitMap.ContainsKey(owner.Position + hexDraft.Key))
                Map.HexUnitMap[owner.Position + hexDraft.Key].RemoveTriggers(owner);
    }
}