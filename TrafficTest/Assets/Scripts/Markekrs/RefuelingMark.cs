using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RefuelingMark : MonoBehaviour, IHexTrigger<Car>
{
    //List of all gas stations on this refueling station
    private GasStationMarker[][] _gasStations;

    private void Awake()
    {
        var gasStationsList = transform.parent.GetComponentsInChildren<GasStationMarker>().
            Select(gasStation => gasStation).ToList();
        var groups = gasStationsList.GroupBy(station => station.LocalPosition.X).ToArray();
        _gasStations = new GasStationMarker[groups.Length][];
        for (var i = 0; i < groups.Length; i++)
            _gasStations[i] = groups[i].AsEnumerable().ToArray();
    }

    public bool Trigger(Car car, EventArgs args = null)
    {
        if (car.IsFilledUp) return true;
        var path = GetFreeSlot(car).Skip(1);
        var enumerable = path as Hex[] ?? path.ToArray();
        if (!enumerable.Any()) return false;
        car.Move(enumerable);
        return true;
    }
    /// <summary>
    /// Creates a path to the nearest free gas stations
    /// </summary>
    /// <param name="car"></param>
    /// <returns></returns>
    private IEnumerable<Hex> GetFreeSlot(Car car)
    {
        foreach (var stationArr in _gasStations)
            foreach (var station in stationArr)
                if (station.Car == null)
                {
                    station.Car = car;
                    return CarsController.Instance.Dijkstras.
                        shortest_path(car.Position, station.transform.position.ToHex());
                }
        return new Hex[0];
    }
}
