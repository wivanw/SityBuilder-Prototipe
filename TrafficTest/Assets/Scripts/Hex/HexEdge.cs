using System;

public struct HexEdge
{
    public readonly Hex A;
    public readonly Hex B;

    public HexEdge(Hex a, Hex b)
    {
        A = a;
        B = b;
    }

    private static readonly Func<HexEdge, HexEdge, bool> EqualsFunc = (firstIn, secondIn) =>
        firstIn.A.X == secondIn.A.X && firstIn.A.Y == secondIn.A.Y &&
        firstIn.B.X == secondIn.B.X && firstIn.B.Y == secondIn.B.Y;

    public static bool operator ==(HexEdge first, HexEdge second)
    {
        return EqualsFunc(first, second) || EqualsFunc(second, first);
    }

    public static bool operator !=(HexEdge first, HexEdge second)
    {
        return !(first == second);
    }

    public bool Equals(HexEdge other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        return (HexEdge)obj == this;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (A.GetHashCode() * 397) ^ B.GetHashCode();
        }
    }
}
