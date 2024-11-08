using System;
using UnityEngine;

public class PlacedBuildingBase : MonoBehaviour
{
    [SerializeField] private BuildingData _scriptedObjectData;
    public BuildingData _buildingData => _scriptedObjectData;

    private int _currentHP;

    private int _buildingLvl;

    protected PlayerBuildingsManager Manager;
    protected Player Owner;

    private void Start()
    {
        _currentHP = _scriptedObjectData.MaxHp[0];
        _buildingLvl = 1;
    }
    public void SetManager(PlayerBuildingsManager manager, ref Action onTick, Player owner)
    {
        Manager = manager;
        onTick += Tick;
        Owner = owner;
    }
    public int GetFaction()
    {
        return Owner.PlayerFaction;
    }
    public void CalculateDamage(int damageReceived)
    {
        damageReceived -= _scriptedObjectData.Armor;
        TakeDamage(damageReceived);
    }
    private void TakeDamage(int damageTaken)
    {
        _currentHP -= damageTaken;
    }

    public void CanLvlUp()
    {
        _scriptedObjectData.CanLevelUp(_buildingLvl);
    }
    protected virtual void Tick()
    {

    }
    public virtual void OnPlaced() { }
    public virtual void OnRemoved() { }
}

