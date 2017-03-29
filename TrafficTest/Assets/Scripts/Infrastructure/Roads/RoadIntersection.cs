using System.Collections.Generic;
using UnityEngine;

public class RoadIntersection : Road
{
    private static Road _prefab;
    public static Road Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<RoadIntersection>(Constants.PathRoadIntersection)); }
    }
    private IEnumerable<RoadHex> _roadOrientations;
    public override IEnumerable<RoadHex> RoadOrientations
    {
        get { return _roadOrientations ?? (_roadOrientations = DraftHelper.IntersectionOrientations()); }
        set { _roadOrientations = value; }
    }
}
