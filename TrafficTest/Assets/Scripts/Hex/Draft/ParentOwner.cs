using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Class implementing the functionality associated with the drafting object
/// </summary>
public abstract class ParentOwner : MonoBehaviour, IHexOwner
{
    // Head hex this object
    public virtual Hex Position { get; set; }
    //General orientation object in the world space
    public Orientation Orientation { get; private set; }
    // Hex wall draft (prohibitions of movement between hexes)
    //public Dictionary<Hex, Orientation> WallDraft { get; set; }
    //Draft object
    private Dictionary<Hex, Biom> _bottomDraft;
    public virtual Dictionary<Hex, Biom> BottomDraft
    {
        get{return _bottomDraft ?? (_bottomDraft = new Dictionary<Hex, Biom>());}
        protected set { _bottomDraft = value; }
    }

    /// <summary>
    /// Here it is necessary to initiate HexOwnerDraft
    /// </summary>
    public void Init()
    {
        Orientation = default(Orientation);
        foreach (var child in GetComponentsInChildren<ChildrenOwner>())
        {
            foreach (var pair in child.BottomDraft)
                BottomDraft.Add(pair.Key + child.LocalPosition, pair.Value);
        }
    }

    public void MoveTransform()
    {
        transform.position = new Vector3(Position.X, transform.position.y, Position.Y);
    }

    /// <summary>
    /// Rotate object in the world space
    /// </summary>
    /// <param name="newOrientation"></param>
    public virtual void SetOrientation(Orientation newOrientation)
    {
        if (Orientation == newOrientation)
            return;
        _bottomDraft = TurnDraft(Orientation, newOrientation, BottomDraft);
        //TurnWalls(newOrientation);
        Map.RotateTransform(transform, Orientation, newOrientation);
        Orientation = newOrientation;
    }

    /// <summary>
    /// Returns a copy drafting object with a specific type rotation
    /// </summary>
    /// <param name="oldOrientation">Initial orientation in the world space</param>
    /// <param name="newOrientation">Final orientation in the world space</param>
    /// <param name="bottomDraft">Draft object</param>
    /// <returns></returns>
    public static Dictionary<Hex, Biom> TurnDraft(Orientation oldOrientation, Orientation newOrientation, Dictionary<Hex, Biom> bottomDraft)
    {
        switch (((int)newOrientation + 3 - (int)oldOrientation) % 4)
        {
            case 0:
                return bottomDraft.ToDictionary(pair => new Hex(pair.Key.Y, -pair.Key.X - 1), pair => pair.Value);
            case 1:
                return bottomDraft.ToDictionary(pair => new Hex(-pair.Key.X - 1, -pair.Key.Y - 1), pair => pair.Value);
            case 2:
                return bottomDraft.ToDictionary(pair => new Hex(-pair.Key.Y - 1, pair.Key.X), pair => pair.Value);
            default:
                return bottomDraft;
        }
    }

    //private void TurnWalls(Orientation newOrientation)
    //{
    //    if (Orientation == newOrientation || WallDraft == null)
    //        return;
    //    var coef = ((int)newOrientation + 4 - (int)Orientation) % 4;
    //    switch (((int)Orientation + 3 - (int)newOrientation) % 4)
    //    {
    //        case 0:
    //            WallDraft = WallDraft.ToDictionary(pair => new Hex(pair.Key.Y, -pair.Key.X), pair => (Orientation)((coef + (int)pair.Value) % 4));
    //            break;
    //        case 1:
    //            WallDraft = WallDraft.ToDictionary(pair => new Hex(pair.Key.Y, -pair.Key.X), pair => (Orientation)((coef + (int)pair.Value) % 4));
    //            break;
    //        case 2:
    //            WallDraft = WallDraft.ToDictionary(pair => new Hex(pair.Key.Y, -pair.Key.X), pair => (Orientation)((coef + (int)pair.Value) % 4));
    //            break;
    //        default:
    //            WallDraft = WallDraft;
    //            break;
    //    }
    //}
}