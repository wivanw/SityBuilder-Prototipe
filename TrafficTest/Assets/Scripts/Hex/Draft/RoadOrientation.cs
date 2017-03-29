using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Class realizing functional of the road marking
/// </summary>
public abstract class RoadOrientation : ParentOwner, IRoad
{
    //Drafting road marking object
    public virtual IEnumerable<RoadHex> RoadOrientations { get; set; }
    /// <summary>
    /// Adds a rotation to the main rotation object all road markings 
    /// in the world space in a given direction
    /// </summary>
    /// <param name="newOrientation">Direction final position</param>
    public override void SetOrientation(Orientation newOrientation)
    {
        RoadOrientations = TurnRoadOrientations(newOrientation, RoadOrientations);
        base.SetOrientation(newOrientation);
    }

    protected IEnumerable<RoadHex> TurnRoadOrientations(Orientation newOrientation, IEnumerable<RoadHex> roadOrientations)
    {
        if (Orientation == newOrientation)
            return roadOrientations;
        switch (((int)newOrientation + 3 - (int)Orientation) % 4)
        {
            case 0:
                return roadOrientations.Select(roadHex => new RoadHex(
                    new Hex(roadHex.Hex.Y, -roadHex.Hex.X - 1),
                    roadHex.RoadOrientations.Select(or => (Orientation)((1 + (int)or) % 4))));
            case 1:
                return roadOrientations.Select(roadHex => new RoadHex(
                    new Hex(-roadHex.Hex.X - 1, -roadHex.Hex.Y - 1),
                    roadHex.RoadOrientations.Select(or => (Orientation)((2 + (int)or) % 4))));
            case 2:
                return roadOrientations.Select(roadHex => new RoadHex(
                    new Hex(-roadHex.Hex.Y - 1, roadHex.Hex.X),
                    roadHex.RoadOrientations.Select(or => (Orientation)((3 + (int)or) % 4))));
            default:
                return roadOrientations;
        }
    }
}
