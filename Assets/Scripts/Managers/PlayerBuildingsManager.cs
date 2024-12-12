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
    }
    public void RemoveBuilding(PlacedBuildingBase placedBuilding)
    {
        _ownedBuildings.Remove(placedBuilding);
    }
}
