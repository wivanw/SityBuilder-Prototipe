using UnityEngine;

public static class Utilities
{
    public static Hex ToHex(this Vector3 position)
    {
        return Map.Instance.Layout.Vector3ToHex(position);
    }
}