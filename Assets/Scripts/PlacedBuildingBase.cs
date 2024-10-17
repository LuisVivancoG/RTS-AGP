using UnityEngine;

public class PlacedBuildingsBase : MonoBehaviour
{
    [SerializeField] private BuildingData _scriptedObjectData;

    private int _currentHP;

    private int _buildingLvl;

    private void Start()
    {
        _currentHP = _scriptedObjectData.MaxHp[0];
        _buildingLvl = 1;
    }

    public void GetBuildingFromPool()
    {

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
}

