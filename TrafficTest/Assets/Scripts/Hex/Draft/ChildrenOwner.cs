using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class involved in creating the parent object drafting
/// </summary>

[Serializable]
public class ChildrenOwner : MonoBehaviour, IHexOwner
{
    public Hex LocalPosition;
    [SerializeField]
    public Biom Biom;
    private Dictionary<Hex, Biom> _bottomDraft;
    public Dictionary<Hex, Biom> BottomDraft
    {
        get { return _bottomDraft ?? (_bottomDraft = DraftHelper.HexDraft(Biom)); }
        protected set { _bottomDraft = value; }
    }
}
