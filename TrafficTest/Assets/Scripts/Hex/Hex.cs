using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Hex
{
    public static Hex One = new Hex(1, 1);
    [SerializeField]
    public int X;
    [SerializeField]
    public int Y;

    public Hex(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static List<Hex> Directions = new List<Hex> { new Hex(0, 1), new Hex(1, 0), new Hex(0, -1), new Hex(-1, 0) };

    public static Hex Neighbor(Hex hex, int direction)
    {
        return hex + Directions[direction];
    }

    public static IEnumerable<Hex> Neighbors(Hex hex)
    {
        return Directions.Select(direction => hex + direction);
    }

    //public static List<Hex> Diagonals = new List<Hex> { new Hex(2, -1), new Hex(1, -2), new Hex(-1, -1), new Hex(-2, 1), new Hex(-1, 2), new Hex(1, 1) };

    //public static Hex DiagonalNeighbor(Hex hex, int direction)
    //{
    //    return Add(hex, Diagonals[direction]);
    //}

    public static int Distance(Hex hex1, Hex hex2)
    {
        return Mathf.Abs(hex1.X - hex2.X) + Mathf.Abs(hex1.Y - hex2.Y);
    }

    public int[] GetArray()
    {
        return new[] { X, Y };
    }

    public static Hex operator *(Hex a, int k)
    {
        return new Hex(a.X * k, a.Y * k);
    }

    public static Hex operator -(Hex hex1, Hex hex2)
    {
        return new Hex(hex1.X - hex2.X, hex1.Y - hex2.Y);
    }

    public static Hex operator +(Hex a, Hex b)
    {
        return new Hex(a.X + b.X, a.Y + b.Y);
    }

    public static bool operator ==(Hex hex1, Hex hex2)
    {
        return hex1.X == hex2.X && hex1.Y == hex2.Y;
    }

    public static bool operator !=(Hex value1, Hex value2)
    {
        return value1.X != value2.X || value1.Y != value2.Y;
    }

    public override string ToString()
    {
        return base.ToString() + string.Format(":[{0},{1}]", X, Y);
    }

    public bool Equals(Hex other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        return obj is Hex && Equals((Hex)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }
}
