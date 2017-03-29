using System;
using System.Linq;

public static class Constants
{
    public const int RoadEntryCount = 3;
    public const int RefuelingCount = 3;
    public const int HexEdgeSize = 1;
    public const int WorldSize = 50;
    public const int BuildGenMargin = 2;
    public const int CarSpeed = 2;
    public const int MaxCarFrequencyGen = 30;
    public const int MaxRefuelingTime = 30;


    private const string PathBuildings = "Prefabs/Buildings/";
    private const string PathRoads = "Prefabs/Roads/";
    private const string PathMarkers = "Prefabs/Markers/";
    public const string PathRefueling = PathBuildings + "Refueling_0";
    public const string PathCarSpawner = PathMarkers + "CarSpawner";
    public const string PathDeparture = PathMarkers + "Departure";
    public const string PathHalfRoadPlane = PathRoads + "AddjoiningRoad";
    public const string PathRoadIntersection = PathRoads + "RoadIntersection";
    public const string PathRoadSimple = PathRoads + "RoadSimple";
    public const string PathRoadTurn = PathRoads + "RoadTurn";
    public const string PathRoadTshaped = PathRoads + "RoadTshaped";
    public const string PathCar = "Prefabs/Cars";

    public const string SelectableShader = "Outlined/Silhouetted Diffuse";
    public const string OutlineColor = "_OutlineColor";

    public static Orientation[] OrientationValues = Enum.GetValues(typeof(Orientation)).Cast<Orientation>().ToArray();
    public static Hex[] RoadDraft = { new Hex(0, -1), new Hex(-1, 0), new Hex(0, 0), new Hex(-1, -1) };
}