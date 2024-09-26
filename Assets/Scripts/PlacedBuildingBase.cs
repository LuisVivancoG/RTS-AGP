using UnityEngine;

public class PlacedBuildingsBase : MonoBehaviour
{
    [SerializeField] private BuildingData ScriptedObjectData;

    private int CurrentHP;

    private int BuildingLvl;

    private void Start()
    {
        CurrentHP = ScriptedObjectData.MaxHp[0];
        BuildingLvl = 1;
    }

    public void GetBuildingFromPool()
    {

    }

    public void CalculateDamage(int damageReceived)
    {
        damageReceived -= ScriptedObjectData.Armor;
        TakeDamage(damageReceived);
    }
    private void TakeDamage(int damageTaken)
    {
        CurrentHP -= damageTaken;
    }

    public void CanLvlUp()
    {
        ScriptedObjectData.CanLevelUp(BuildingLvl);
    }
}

