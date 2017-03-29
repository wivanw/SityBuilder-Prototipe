using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part of the road is one hex. Is the child of the parent object.
/// </summary>
public class AdjoiningFourthRoad : ChildrenOwner, IRoad
{
    [SerializeField]
    public Orientation[] RoadOrientation;

    private IEnumerable<RoadHex> _roadOrientations;
    public IEnumerable<RoadHex> RoadOrientations
    {
        get
        {
            return _roadOrientations ?? (_roadOrientations = new[] { new RoadHex(LocalPosition, RoadOrientation) });
        }
    }
}