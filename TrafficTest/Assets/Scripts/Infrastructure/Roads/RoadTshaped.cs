using System.Collections.Generic;
using UnityEngine;

public class RoadTshaped : Road
{
    private static Road _prefab;
    public static Road Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<RoadTshaped>(Constants.PathRoadTshaped)); }
    }
    private IEnumerable<RoadHex> _roadOrientations;
    public override IEnumerable<RoadHex> RoadOrientations
    {
        get { return _roadOrientations ?? (_roadOrientations = DraftHelper.TsharperOrientations()); }
        set { _roadOrientations = value; }
    }
}
