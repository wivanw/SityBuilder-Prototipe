using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class BuildingController
{
    public BuildingController()
    {
        RefuelingGen();
    }
    /// <summary>
    /// Generates refueling buildings on the map
    /// </summary>
    private void RefuelingGen()
    {
        var refueling = Refueling.Prefab;
        BuildingsGen(refueling, Constants.RefuelingCount);
    }

    private void BuildingsGen<T>(T building, int count) where T : ParentOwner
    {
        Func<int> rnd = () => Random.Range(Constants.BuildGenMargin, Constants.WorldSize - Constants.BuildGenMargin) / 2 * 2 + 1;
        var buildings = new T[count];
        for (var i = 0; i < count; i++)
        {
            var newBuilding = Object.Instantiate(building.gameObject).GetComponent<T>();
            newBuilding.Init();
            Hex position;
            var orientation = default(Orientation);
            var isGen = true;
            var counterCircle = 0;
            do
            {
                position = new Hex(rnd(), rnd());
                foreach (var tempOr in Constants.OrientationValues)
                {
                    if (MapManager.Instance.Map.IsOccupied(ParentOwner.TurnDraft(newBuilding.Orientation, tempOr, newBuilding.BottomDraft), position) ||
                        !RoadController.Instance.PathFindingTest(newBuilding, position, tempOr))
                        continue;
                    orientation = tempOr;
                    isGen = false;
                    break;
                }
                if (counterCircle++ > 50)
                {
                    Debug.LogError("Failed build building. Try again.");
                    Application.Quit();
                }

            } while (isGen);
            buildings[i] = NewBuilding(newBuilding, position, orientation);
        }
    }

    private static T NewBuilding<T>(T building, Hex position, Orientation orientation) where T : ParentOwner
    {
        building.Position = position;
        building.MoveTransform();
        building.SetOrientation(orientation);
        MapManager.Instance.Map.SetNewOwner(building);
        return building;
    }
}