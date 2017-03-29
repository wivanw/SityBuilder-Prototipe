using System.Collections.Generic;

public abstract class Road : RoadOrientation
{
    private Dictionary<Hex, Biom> _bottomDraft;
    public override Dictionary<Hex, Biom> BottomDraft
    {
        get { return _bottomDraft ?? (_bottomDraft = DraftHelper.Road()); }
        protected set { _bottomDraft = value; }
    }
    
    public override IEnumerable<RoadHex> RoadOrientations { get; set; }
}