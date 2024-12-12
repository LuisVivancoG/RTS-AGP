using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int _playerIndex;
    private int _playerFaction;
    public int PlayerFaction => _playerFaction;

    private float _storedFood;
    private PlayerBuildingsManager _buildingManager;

    public Action<float> OnFoodChanged;

    public PlayerBuildingsManager BuildingManager => _buildingManager;

    public Player(int playerIndex, int playerFaction, GameManager gameManager)
    {
        _playerIndex = playerIndex;
        _playerFaction = playerFaction;
        _storedFood = 500;

        _buildingManager = new PlayerBuildingsManager(this, gameManager);
    }

    public void ResourceGain(float gain)
    {
        _storedFood += gain;
        OnFoodChanged?.Invoke(_storedFood);
    }

}
