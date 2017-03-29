using Object = UnityEngine.Object;

public static class RoadDraftHelper
{
    /// <summary>
    /// Identify road type based on the availability of other parts expensive near
    /// </summary>
    /// <param name="neighbors">Pointers to the neighboring parts of the threat if they are</param>
    /// <param name="position">The position of the part of the road that is now being identified</param>
    /// <returns></returns>
    public static Road IdentifyRoadType(Orientation[] neighbors, Hex position)
    {
        switch (neighbors.Length)
        {
            case (1):
                var orientation = neighbors[0] == Orientation.North || neighbors[0] == Orientation.South
                    ? Orientation.North
                    : Orientation.East;
                return CreateRoadObj(RoadType.Simple, orientation, position);
            #region SimpleRoad
            case (2):
                if (neighbors[0] == Orientation.North && neighbors[1] == Orientation.South)
                    return CreateRoadObj(RoadType.Simple, Orientation.North, position);
                if (neighbors[0] == Orientation.East && neighbors[1] == Orientation.West)
                    return CreateRoadObj(RoadType.Simple, Orientation.East, position);
                #endregion SimpleRoad

                #region TurnRoad
                if (neighbors[0] == Orientation.North && neighbors[1] == Orientation.East)
                    return CreateRoadObj(RoadType.Turn, Orientation.North, position);
                if (neighbors[0] == Orientation.East && neighbors[1] == Orientation.South)
                    return CreateRoadObj(RoadType.Turn, Orientation.East, position);
                if (neighbors[0] == Orientation.South && neighbors[1] == Orientation.West)
                    return CreateRoadObj(RoadType.Turn, Orientation.South, position);
                if (neighbors[0] == Orientation.North && neighbors[1] == Orientation.West)
                    return CreateRoadObj(RoadType.Turn, Orientation.West, position);
                #endregion TurnRoad
                return null;
            case (3):
                #region TshapedRoad
                if (neighbors[0] == Orientation.North && neighbors[1] == Orientation.East && neighbors[2] == Orientation.West)
                    return CreateRoadObj(RoadType.Tshaped, Orientation.North, position);
                if (neighbors[0] == Orientation.North && neighbors[1] == Orientation.East && neighbors[2] == Orientation.South)
                    return CreateRoadObj(RoadType.Tshaped, Orientation.East, position);
                if (neighbors[0] == Orientation.East && neighbors[1] == Orientation.South && neighbors[2] == Orientation.West)
                    return CreateRoadObj(RoadType.Tshaped, Orientation.South, position);
                if (neighbors[0] == Orientation.North && neighbors[1] == Orientation.South && neighbors[2] == Orientation.West)
                    return CreateRoadObj(RoadType.Tshaped, Orientation.West, position);
                return null;
            #endregion TshapedRoad

            case (4):
                return CreateRoadObj(RoadType.Intersection, Orientation.North, position);
            default:
                return null;
        }
    }

    private static Road CreateRoadObj(RoadType type, Orientation orientation, Hex position)
    {
        switch (type)
        {
            case RoadType.Simple:
                return InstantiateRoadObj(RoadSimple.Prefab, orientation, position);
            case RoadType.Turn:
                return InstantiateRoadObj(RoadTurn.Prefab, orientation, position);
            case RoadType.Tshaped:
                return InstantiateRoadObj(RoadTshaped.Prefab, orientation, position);
            case RoadType.Intersection:
                return InstantiateRoadObj(RoadIntersection.Prefab, orientation, position);
            default:
                return null;
        }
    }

    private static Road InstantiateRoadObj(Road road, Orientation orientation, Hex position)
    {
        var obj = Object.Instantiate(road);
        obj.Position = RoadController.Unscaled(position);
        obj.MoveTransform();
        obj.SetOrientation(orientation);
        MapManager.Instance.Map.SetNewOwner(obj);
        obj.transform.SetParent(RoadPool.Instance.transform);
        return obj;
    }
}
