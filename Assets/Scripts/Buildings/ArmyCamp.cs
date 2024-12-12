using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCamp : PlacedBuildingBase
{
    //[SerializeField] private int[] _additionalArmyUnits;
    //[SerializeField] private int _spawnUnits;
    //[SerializeField] private int _unitsPerCycle;
    //[SerializeField] private TroopType _troopSpawned;

    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private Animator _buildingAnim;
    private Queue<UnitsBase> _unitsRequested = new Queue<UnitsBase>();
    private UnitsManager _unitsManager;
    private bool _isSpawning = false;

    private void Awake()
    {
        _unitsManager = FindAnyObjectByType<UnitsManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("bang bang");
            CalculateDamage(6f);
        }
        if (!_isSpawning && _unitsRequested.Count > 0)
        {
            StartCoroutine(SpawnUnit());
        }
    }

    /*void OnSpawning(UnitsBase unitQueue)
    {
        _unitsManager.GenerateUnits(1, unitQueue, _spawnPoint, ref _unitsManager._unitsListA);
        _buildingAnim.SetTrigger("Spawning");
    }*/

    public void RequestUnit(UnitsBase unit)
    {
        _unitsRequested.Enqueue(unit); // Add unit to the queue
    }

    private IEnumerator SpawnUnit()
    {
        _isSpawning = true;

        UnitsBase unitToSpawn = _unitsRequested.Dequeue();
        _unitsManager.GenerateUnits(1, unitToSpawn, _spawnPoint.transform, ref _unitsManager._unitsListA);
        _buildingAnim.SetTrigger("Spawning");

        yield return new WaitForSeconds(2.0f);

        _isSpawning = false;
    }

    public override void Initialize()
    {
        base.Initialize();
        _isSpawning = false;
    }

    public override void CalculateDamage(float damageReceived)
    {
        base.CalculateDamage(damageReceived);
        if (_currentHP <= 0 && !_isDestroyed)
        {
            _isDestroyed = true;
            BuildingPlacementManager.Instance.RemoveBuilding(this);
        }
    }

    public enum TroopType
    {
        None = 0,
        Base = 1,
        WaterProof = 2,
        HeavyWeight = 3,
    }
}
