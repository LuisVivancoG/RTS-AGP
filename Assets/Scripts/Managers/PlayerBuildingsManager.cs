using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildingsManager
{
    private Player _owner;
    private GameManager _gameManager;
    private List<PlacedBuildingBase> _ownedBuildings = new();
    private Action _tick;

    public PlayerBuildingsManager(Player owner, GameManager gameManager)
    {
        _owner = owner;
        _gameManager = gameManager;
    }

    public void AddBuilding(PlacedBuildingBase placedBuilding)
    {
        _ownedBuildings.Add(placedBuilding);
        placedBuilding.SetManager(this, ref _tick, _owner);
        var cellsBuildingIsOn = _gameManager.GameGrid.GetCellsAroundPosition(placedBuilding.transform.position, placedBuilding._buildingData.BuildingSize);
        foreach (var cell in cellsBuildingIsOn)
        {
            cell.AddBuildingToCell(placedBuilding);
        }
        placedBuilding.OnPlaced();
    }
    public void RemoveBuilding(PlacedBuildingBase placedBuilding)
    {
        _ownedBuildings.Remove(placedBuilding);
        var cellsBuildingIsOn = _gameManager.GameGrid.GetCellsAroundPosition(placedBuilding.transform.position, placedBuilding._buildingData.BuildingSize);
        foreach (var cell in cellsBuildingIsOn)
        {
            cell.RemoveBuildingFromCell(placedBuilding);
        }
        placedBuilding.OnRemoved();
    }
    public void OnUpdate()
    {
        _tick?.Invoke();
    }

}
