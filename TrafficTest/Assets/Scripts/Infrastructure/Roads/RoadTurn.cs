using System.Collections.Generic;
using UnityEngine;

public class RoadTurn : Road
{
    private static Road _prefab;
    public static Road Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<RoadTurn>(Constants.PathRoadTurn)); }
    }
    private IEnumerable<RoadHex> _roadOrientations;
    public override IEnumerable<RoadHex> RoadOrientations
    {
        get { return _roadOrientations ?? (_roadOrientations = DraftHelper.TurnOrientations()); }
        set { _roadOrientations = value; }
    }
}
