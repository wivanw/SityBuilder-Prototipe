using System;
using UnityEngine;

public class Departure : MonoBehaviour, IHexTrigger<Car>
{
    private static Departure _prefab;
    public static Departure Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<Departure>(Constants.PathDeparture)); }
    }

    public bool Trigger(Car target, EventArgs args = null)
    {
        if (target.TargetFollowing == transform.position.ToHex())
            Destroy(target.gameObject);
        return true;
    }

    private void Start()
    {
        Map.Instance.HexUnitMap[transform.position.ToHex()].AddTrigger(this);
    }
}
