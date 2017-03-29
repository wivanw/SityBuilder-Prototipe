using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GasStationMarker : AdjoiningFourthRoad, IHexTrigger<Car>
{
    private Departure[] _departures;
    private Departure[] Departures { get { return _departures ?? (_departures = FindObjectsOfType<Departure>()); } }
    public Car Car;

    private void Start()
    {
        Map.Instance.HexUnitMap[transform.position.ToHex()].AddTrigger(this);
    }

    public bool Trigger(Car target, EventArgs args = null)
    {
        if (!target.Equals(Car) || Car.IsFilledUp)
            return true;
        Car.IsFilledUp = true;
        StartCoroutine(RefillingTimer());
        return false;
    }

    public IEnumerator RefillingTimer()
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
