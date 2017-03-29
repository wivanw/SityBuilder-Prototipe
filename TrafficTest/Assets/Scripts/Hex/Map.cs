using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map
{
    public static Map Instance { get; private set; }
    //The main game map containing all the terrain drawings, 
    //infrastructure, moving objects, as well as all the triggers
    public Dictionary<Hex, HexUnit> HexUnitMap { get; private set; }
    public Layout Layout;
    //List of biomes that are considered to be occupied by default
    private static readonly Biom[] DefaultOccupiedBioms = { Biom.Forest, Biom.Building, Biom.Road, Biom.Occupied };

    public Map(int worldSize)
    {
        Instance = this;
        Layout = new Layout(Constants.HexEdgeSize);
        HexUnitMap = new Dictionary<Hex, HexUnit>();
        GenerateNewMap(worldSize + 1);
    }

    public void GenerateNewMap(int worldSize)
    {
        for (var x = 0; x < worldSize; x++)
            for (var y = 0; y < worldSize; y++)
                HexUnitMap.Add(new Hex(x, y), new HexUnit(Biom.Plain));
    }
    /// <summary>
    /// Adds new objects to the main map
    /// </summary>
    /// <typeparam name="T">Тип обьекта реализующий функционал владения гексами</typeparam>
    /// <param name="owners"></param>
    public void SetNewOwner<T>(params T[] owners) where T : ParentOwner
    {
        foreach (var owner in owners)
        {
            foreach (var pair in owner.BottomDraft)
                HexUnitMap[pair.Key + owner.Position] = new HexUnit(pair.Value);

            var orientation = owner as RoadOrientation;
            if (orientation != null)
                foreach (var roadOrientation in orientation.RoadOrientations)
                    HexUnitMap[roadOrientation.Hex + owner.Position].OtherObjects[HexUnitObj.RoadOrientation] = roadOrientation.RoadOrientations;
        }
    }
    /// <summary>
    /// Whether the territory on the main map is free for placing this object
    /// </summary>
    /// <param name="draft">Draft object</param>
    /// <param name="position">Desired position</param>
    /// <param name="types">Types of biomes that are regarded as occupied</param>
    /// <returns>Is this territory free</returns>
    public bool IsOccupied(IDictionary<Hex, Biom> draft, Hex position, Biom[] types = null)
    {
        return draft.Keys.Select(hex => hex + position).Any(hex => IsOccupied(hex, types));
    }
    /// <summary>
    /// Whether the territory on the main map is free for placing this object
    /// </summary>
    /// <param name="hexes">Whether the territory on the main map is free on the hex list</param>
    /// <param name="types">Types of biomes that are regarded as occupied</param>
    /// <returns>Is this territory free</returns>
    public bool IsOccupied(IEnumerable<Hex> hexes, Biom[] types = null)
    {
        return hexes.Any(hex => IsOccupied(hex, types));
    }
    /// <summary>
    /// Whether the territory on the main map is free on current hex
    /// </summary>
    /// <param name="hex">Position of the test</param>
    /// <param name="types">Types of biomes that are regarded as occupied</param>
    /// <returns>Is this hex free</returns>
    public bool IsOccupied(Hex hex, Biom[] types = null)
    {
        return !HexUnitMap.ContainsKey(hex) || (types ?? DefaultOccupiedBioms).Any(type => HexUnitMap[hex].Type == type);
    }
    /// <summary>
    /// Rotate transform object
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="oldOrientation">The current orientation of the object in the world space</param>
    /// <param name="newOrientation">New desirable orientation in the world space</param>
    public static void RotateTransform(Transform transform, Orientation oldOrientation, Orientation newOrientation)
    {
        if (oldOrientation == newOrientation)
            return;
        var angle = ((int)newOrientation + 4 - (int)oldOrientation) % 4 * 90f;
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public IEnumerator GetEnumerator()
    {
        return HexUnitMap.GetEnumerator();
    }


}
public enum Orientation { North, East, South, West }