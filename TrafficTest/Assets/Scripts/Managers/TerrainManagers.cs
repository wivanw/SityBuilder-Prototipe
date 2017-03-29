using System.Linq;
using UnityEngine;

public class TerrainManagers : MonoBehaviour
{
    public static TerrainManagers Instance { get; private set; }
    private Terrain _terrain;

    private void Awake()
    {
        Instance = this;
    }

    public void InitTerrain(Map map)
    {
        _terrain = Terrain.activeTerrain;
        if (_terrain == null)
            return;
        var terrainPos = _terrain.transform.position;
        var size = _terrain.terrainData.size;
        var keys = _terrain.terrainData.treeInstances.
            Select(tree => Vector3.Scale(tree.position, size) + terrainPos).
            Select(position => position.ToHex() - Hex.One).
            Where(key => map.HexUnitMap.ContainsKey(key));
        foreach (var key in keys)
            map.HexUnitMap[key] = new HexUnit(Biom.Forest);
    }
}
