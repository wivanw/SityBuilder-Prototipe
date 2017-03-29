using System.Collections.Generic;

public class Autobus : Car
{
    private Dictionary<Hex, Biom> _bottomDraft;
    public override Dictionary<Hex, Biom> BottomDraft
    {
        get { return _bottomDraft ?? (_bottomDraft = DraftHelper.Autobus()); }
        protected set { _bottomDraft = value; }
    }
}
