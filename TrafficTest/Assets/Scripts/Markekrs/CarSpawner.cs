using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : ParentOwner
{
    private static CarSpawner _prefab;
    public static CarSpawner Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<CarSpawner>(Constants.PathCarSpawner)); }
    }

    private Dictionary<Hex, Biom> _bottomDraft;
    public override Dictionary<Hex, Biom> BottomDraft
    {
        get { return _bottomDraft ?? (_bottomDraft = DraftHelper.Road()); }
        protected set { _bottomDraft = value; }
    }
}
