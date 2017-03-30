using System;
using UnityEngine;

/// <summary>
/// Departure from the city. 
/// </summary>
public class Departure : MonoBehaviour, IHexTrigger<Car>
{
    private static Departure _prefab;
    public static Departure Prefab
    {
        get { return _prefab ?? (_prefab = Resources.Load<Departure>(Constants.PathDeparture)); }
    }
    /// <summary>
    /// Removes a movables object if departure is ultimate goal movement object
    /// </summary>
    /// <param name="target">Checked object</param>
    /// <param name="args"></param>
    /// <returns>Is it allowed to move object to the position departure</returns>
#warning need implement pool-object functional
    public bool Trigger(Car target, EventArgs args = null)
    {
        if (target.TargetFollowing == transform.position.ToHex())
            Destroy(target.gameObject);
        return true;
    }
    /// <summary>
    /// Adds the departure trigger to the main map
    /// </summary>
    private void Start()
    {
        Map.Instance.HexUnitMap[transform.position.ToHex()].AddTrigger(this);
    }
}
