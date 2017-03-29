using System.Collections.Generic;

public static class DraftHelper
{
    //____________________________
    //This is a temporary class until the implementation of the Editor drafting tool
    //____________________________

    public static Dictionary<Hex, Biom> HexDraft(Biom biom)
    {
        return new Dictionary<Hex, Biom> { { new Hex(0, 0), biom } };
    }

    public static Dictionary<Hex, Biom> Road()
    {
        return new Dictionary<Hex, Biom>
        {
            { new Hex(0, 0), Biom.Road },
            { new Hex(-1, 0), Biom.Road },
            { new Hex(0, -1), Biom.Road },
            { new Hex(-1, -1), Biom.Road }

        };
    }

    public static Dictionary<Hex, Biom> Car()
    {
        return new Dictionary<Hex, Biom>
        {
            { new Hex(0, 0), Biom.Occupied }
        };
    }

    public static Dictionary<Hex, Biom> Autobus()
    {
        return new Dictionary<Hex, Biom>
        {
            { new Hex(0, 0), Biom.Occupied }
        };
    }

    public static IEnumerable<RoadHex> IntersectionOrientations()
    {
        return new[]
        {
            new RoadHex(new Hex(0, 0), new[] {Orientation.North, Orientation.West}),
            new RoadHex(new Hex(-1, 0), new[] {Orientation.South, Orientation.West}),
            new RoadHex(new Hex(0, -1), new[] {Orientation.North, Orientation.East}),
            new RoadHex(new Hex(-1, -1), new[] {Orientation.South, Orientation.East})
        };
    }

    public static IEnumerable<RoadHex> SimpleOrientations()
    {
        return new[]
        {
            new RoadHex(new Hex(0, 0), new[] {Orientation.North}),
            new RoadHex(new Hex(-1, 0), new[] {Orientation.South}),
            new RoadHex(new Hex(0, -1), new[] {Orientation.North}),
            new RoadHex(new Hex(-1, -1), new[] {Orientation.South})
        };
    }

    public static IEnumerable<RoadHex> TsharperOrientations()
    {
        return new[]
        {
            new RoadHex(new Hex(0, 0), new[] {Orientation.West, Orientation.North}),
            new RoadHex(new Hex(-1, 0), new[] {Orientation.South, Orientation.West, }),
            new RoadHex(new Hex(0, -1), new[] {Orientation.North, Orientation.East, }),
            new RoadHex(new Hex(-1, -1), new[] {Orientation.East})
        };
    }

    public static IEnumerable<RoadHex> TurnOrientations()
    {
        return new[]
        {
            new RoadHex(new Hex(0, 0), new[] {Orientation.North}),
            new RoadHex(new Hex(-1, 0), new[] {Orientation.South}),
            new RoadHex(new Hex(0, -1), new[] {Orientation.East}),
            new RoadHex(new Hex(-1, -1), new[] {Orientation.East})
        };
    }
}

public enum DraftUnitType { Refueling, RoadFourth, Car, AdjoiningRoad, RoadIntersection }