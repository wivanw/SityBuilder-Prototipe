using System.Collections.Generic;

public interface IRoad
{
    IEnumerable<RoadHex> RoadOrientations { get; }
}

public struct RoadHex
{
    public Hex Hex;
    public IEnumerable<Orientation> RoadOrientations;
    public RoadHex(Hex hex, IEnumerable<Orientation> orientations)
    {
        Hex = hex;
        RoadOrientations = orientations;
    }
}