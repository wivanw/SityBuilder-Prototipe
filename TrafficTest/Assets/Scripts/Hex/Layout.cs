using UnityEngine;
/// <summary>
/// Class defining the shape, size, orientation of the grid 
/// of hexes in space and the accompanying functional
/// </summary>
public struct Layout
{
    private readonly int _edgeSize;
    public Layout(int edgeSize)
    {
        _edgeSize = edgeSize;
    }

    public Vector3 HexToVector3(Hex hex)
    {
        return new Vector3(hex.X * _edgeSize, 0, hex.Y * _edgeSize);
    }

    public Hex Vector3ToHex(Vector3 point)
    {
        return new Hex(Mathf.CeilToInt(point.x / _edgeSize), Mathf.CeilToInt(point.z / _edgeSize));
    }
}
