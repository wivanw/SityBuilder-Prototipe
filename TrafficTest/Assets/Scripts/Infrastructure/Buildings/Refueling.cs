using System.Linq;
using UnityEngine;

public sealed class Refueling : RoadOrientation
{
    private static Refueling _prefab;
    public static Refueling Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<Refueling>(Constants.PathRefueling)); }
    }

    public void Awake()
    {
        RoadOrientations = new RoadHex[0];
        foreach (var road in GetComponentsInChildren<IRoad>())
            RoadOrientations = RoadOrientations.Concat(road.RoadOrientations);
    }
}