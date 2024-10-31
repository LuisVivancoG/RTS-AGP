using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int _playerIndex;
    private int _playerTeam;
    public int PlayerTeam => _playerTeam;

    private float _storedFood;
    private PlayerBuildingsManager _buildingManager;

    public Action<float> OnPowerChanged;

    public PlayerBuildingsManager BuildingManager => _buildingManager;

    public Player(int playerIndex, int playerFaction, GameManager gameManager)
    {
        _playerIndex = playerIndex;
        _playerTeam = playerFaction;
        _storedFood = 0;

        _buildingManager = new PlayerBuildingsManager(this, gameManager);
    }

    public void ResourceGain(float gain)
    {
        _storedFood += gain;
        OnPowerChanged?.Invoke(_storedFood);
    }

}
