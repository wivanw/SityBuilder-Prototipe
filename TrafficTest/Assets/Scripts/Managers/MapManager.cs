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
    /// <summary>
    /// Visualization road markings as well as planar graph of key points of the map
    /// </summary>
#if UNITY_EDITOR
    private void Update()
    {
        foreach (var edge in RoadController.Edges)
            Debug.DrawLine(new Vector3(edge.A.X, 0.02f, edge.A.Y) * 2 + new Vector3(1, 0, 1), new Vector3(edge.B.X, 0.02f, edge.B.Y) * 2 + new Vector3(1, 0, 1), Color.green);
        //Color[] colors = { Color.cyan, Color.black, Color.blue, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
        //var i = 0;
        //foreach (var nodes in CarsController.Dijkstras._vertices)
        //    foreach (var node in nodes.Value)
        //        Debug.DrawLine(Map.Layout.HexToVector3(nodes.Key), Map.Layout.HexToVector3(node), colors[i++ % colors.Length]);
    }

    /// <summary>
    /// Visualizes draftings objects that are fed to entrance
    /// </summary>
    /// <param name="map">Object under visualization</param>
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
    /// <summary>
    /// Implements functionality of movables objects and it turns out whether the moving succeeded
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner">Object sending interleaving request</param>
    /// <param name="position">Desired position of mixing</param>
    /// <returns></returns>
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

    /// <summary>
    /// Replaces owner of a particular hex and removes traces of old one if any
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner">New owner of hex</param>
    /// <param name="hex">The position of the operation being sold</param>
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

    /// <summary>
    /// Deletes triggers of the object from map
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner">Exposure object</param>
    public void RemoveTrigger<T>(T owner) where T : IHexOwner, IHexTrigger<T>, IMovables
    {
        foreach (var hexDraft in owner.BottomDraft)
            if (Map.HexUnitMap.ContainsKey(owner.Position + hexDraft.Key))
                Map.HexUnitMap[owner.Position + hexDraft.Key].RemoveTriggers(owner);
    }
}