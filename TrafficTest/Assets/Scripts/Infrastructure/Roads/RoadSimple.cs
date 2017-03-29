using System.Collections.Generic;
using UnityEngine;

public class RoadSimple : Road
{
    private static Road _prefab;
    public static Road Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<RoadSimple>(Constants.PathRoadSimple)); }
    }
    private IEnumerable<RoadHex> _roadOrientations;
    public override IEnumerable<RoadHex> RoadOrientations
    {
        get { return _roadOrientations ?? (_roadOrientations = DraftHelper.SimpleOrientations()); }
        set { _roadOrientations = value; }
    }
}
