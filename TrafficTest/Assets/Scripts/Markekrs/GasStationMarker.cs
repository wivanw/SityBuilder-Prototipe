using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GasStationMarker : AdjoiningFourthRoad, IHexTrigger<Car>
{
    // List of all departures from the city
    private Departure[] _departures;
    private Departure[] Departures { get { return _departures ?? (_departures = FindObjectsOfType<Departure>()); } }
    public Car Car;

    private void Start()
    {
        Map.Instance.HexUnitMap[transform.position.ToHex()].AddTrigger(this);
    }
    /// <summary>
    /// Starts car refueling effect if purpose of the car's movement
    ///  is precisely this refueling
    /// </summary>
    /// <param name="target">Car to be inspected</param>
    /// <param name="args"></param>
    /// <returns>Is it allowed to move object to position that gas station</returns>
    public bool Trigger(Car target, EventArgs args = null)
    {
        if (!target.Equals(Car) || Car.IsFilledUp)
            return true;
        Car.IsFilledUp = true;
        StartCoroutine(RefuelingTimer());
        return false;
    }

    /// <summary>
    /// Controls animation of refueling car 
    /// and then sends it to an accidental departure from city
    /// </summary>
    /// <returns></returns>
    public IEnumerator RefuelingTimer()
    {
        Car.StartAnimation();
        yield return new WaitForSeconds(1 + PlayerPrefs.GetFloat(PrefKey.RefuelingTime) * Constants.MaxRefuelingTime);
        Car.StopAnimation();
        var path = CarsController.Instance.Dijkstras.
                        shortest_path(transform.position.ToHex(),
                        Departures[Random.Range(0, Departures.Length - 1)].
                            transform.position.ToHex());
        Car.Move(path);
        Car = null;
    }
}
