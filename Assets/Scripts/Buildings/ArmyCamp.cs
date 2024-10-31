using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCamp : PlacedBuildingBase
{
    [SerializeField] private int[] _additionalArmyUnits;
    [SerializeField] private int _spawnUnits;
    [SerializeField] private int _unitsPerCycle;
    [SerializeField] private TroopType _troopSpawned;

    public enum TroopType
    {
        None = 0,
        Base = 1,
        WaterProof = 2,
        HeavyWeight = 3,
    }
}
