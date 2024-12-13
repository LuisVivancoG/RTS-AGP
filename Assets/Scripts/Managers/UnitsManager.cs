using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsManager : MonoBehaviour //This manager gives orders to the units defined by UnitselectionSystem.
                                          //Between its functions are: generate units on demand, establishes priority of base state machine and
                                          //override FSM by assigning new pathfinding or forcing to stay still.
{
    public GameManager _gameManager { get; private set;}

    [SerializeField] private int _unitsSpawned;

    [SerializeField] private AllUnitsData _allUnitsData;
    public AllUnitsData RunTimeUnitsData => _allUnitsData;

    //[SerializeField] private UnitsBase _unitToSpawnA;
    [SerializeField] private UnitsBase _unitToSpawnB;
    //[SerializeField] private GameObject _spawnerA; //TODO replace spawner with Army camps position and update GenerateUnits method to use that position
    [SerializeField] private GameObject _spawnerB;
    [SerializeField] private GameObject _poolsParent;
    [SerializeField] private int _offsetFromSpawner;
    public List<UnitsBase> _unitsListA = new();
    public List<UnitsBase> _unitsListB = new();
    internal Dictionary<UnitType, UnitsPool> _unitsPools;

    private void Start()
    {
        InitializePool();

        GenerateUnits(2, _unitToSpawnB, _spawnerB.transform, ref _unitsListB);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GenerateUnits(2, _unitToSpawnB, _spawnerB.transform, ref _unitsListB);
        }

        foreach(var unit in _unitsListA)
        {
            /*if (unit.CellUnit._currentDestination != new Vector3())
            {
                return;
            }*/
            if (MovementOrder(unit, unit.CellUnit._currentDestination)) continue;
            if (EnemyCheck(unit)) continue;
            if (BuildingCheck(unit)) continue;
        }

        foreach (var unit in _unitsListB)
        {
            //if (MovementOrder(unit, unit.CellUnit._currentDestination)) continue;
            if (EnemyCheck(unit)) continue;
            if (BuildingCheck(unit)) continue;
        }
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        //GenerateUnits(1, _unitToSpawnA, _spawnerA.transform, ref _unitsListA);
        //GenerateUnits(2, _unitToSpawnB, _spawnerB.transform, ref _unitsListB);
    }

    public void GenerateUnits(int faction, UnitsBase data, Transform spawner, ref List<UnitsBase> list)
    {
        //int mapWidth = _gameManager.GameGrid.Width * _gameManager.GameGrid.CellSize;
        //int mapHeight = _gameManager.GameGrid.Height * _gameManager.GameGrid.CellSize;
        var unit = _unitsPools[data.UnitData.KindOfUnit].Get();
        var spawnerCentered = _gameManager.GameGrid.GetCellWorldCenter(spawner.transform.position);

        unit.transform.position = spawnerCentered;
        unit.CellUnit.Setup(faction, _gameManager.GameGrid);
        list.Add(unit);

        Vector3 initialMovement = new Vector3(0, 0, -((_gameManager.GameGrid.CellSize) * _offsetFromSpawner));

        MovementOrder(unit, spawnerCentered + initialMovement);

        /*for (int i = 0; i < _unitsSpawned; i++)
        {
            UnitsBase cellUnit = Instantiate(data, spawner.transform.position, Quaternion.identity, _poolsParent.transform);

            cellUnit.CellUnit.Setup(faction, i, _gameManager.GameGrid);

            list.Add(cellUnit);
        }*/
    }

    public void LocTarget(CellUnit unitToMove, Vector3 targetDestination)
    {
        if (unitToMove != null && targetDestination != null)
        {
            Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(targetDestination);
            unitToMove.SetNewTarget(cellInGrid);
        }
    }

    private bool EnemyCheck(UnitsBase unit)
    {
        CellUnit closestEnemy = _gameManager.GameGrid.FindClosestOtherFactionUnit(unit.CellUnit, unit.UnitData.MaxDetectionRange);

        if (closestEnemy != null)
        {
            var minDistance = unit.transform.forward + (new Vector3(0, 0, (_gameManager.GameGrid.CellSize * unit.UnitData.MaxAttackRange)));
            //Debug.Log(minDistance);

            unit.CellUnit.SetNewTarget(closestEnemy.transform.position - minDistance);
            //unit.CellUnit.SetEnemyTarget(closestEnemy);
            //Debug.LogWarning($"Unit: {unit} encounterd enemy: {closestEnemy} at {_gameManager.GameGrid.CellIdFromPosition(closestEnemy.transform.position)}");
            return true;
        }
        return false;
    }
    private bool BuildingCheck(UnitsBase unit)
    {
        PlacedBuildingBase closestFoeBuilding = _gameManager.GameGrid.FindClosestEnemyBuilding(unit.CellUnit, unit.UnitData.MaxDetectionRange);

        if (closestFoeBuilding != null)
        {
            var minDistance = unit.transform.forward + (new Vector3 (0,0, (_gameManager.GameGrid.CellSize * unit.UnitData.MaxAttackRange)));
            //Debug.Log(minDistance);

            unit.CellUnit.SetNewTarget(closestFoeBuilding.transform.position - minDistance);
            //Debug.Log($"Unit: {unit} found building: {closestFoeBuilding} moving towards it");
            return true;
        }
        return false;
    }

    private bool MovementOrder(UnitsBase unit, Vector3 target)
    {
        if (Vector3.Distance(unit.transform.position, target) > _gameManager.GameGrid.CellSize)
        {
            Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(target);
            unit.CellUnit.SetNewTarget(cellInGrid);
            //Debug.Log($"Unit: {unit} following requested location {target}");
            return true;
        }
        else
        {
            //unit.CellUnit._currentCommand = Vector3.zero;
            return false;
        }
    }

    internal void InitializePool()
    {

        _unitsPools = new Dictionary<UnitType, UnitsPool>();
        foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
        {
            _unitsPools.Add(type, new UnitsPool(() => CreatePoolUnitType(type), GetUnitFromPool, ReturnUnitToPool));
        }
    }
    private UnitsBase CreatePoolUnitType(UnitType unitType)
    {
        UnitsData dataToUse = GetUnitData(unitType);
        var newPooledUnit = Instantiate(dataToUse.UnitPrefab, _poolsParent.transform);
        newPooledUnit.SetUnitManager(this);

        return newPooledUnit;
    }
    private UnitsData GetUnitData(UnitType unitType)
    {
        return RunTimeUnitsData.Data.FirstOrDefault(b => b.KindOfUnit == unitType);
    }
    private void GetUnitFromPool(UnitsBase unit)
    {
        unit.gameObject.SetActive(true);
    }
    private void ReturnUnitToPool(UnitsBase unit)
    {
        unit.gameObject.SetActive(false);
    }
    internal void UnitDeath(UnitsBase unit)
    {
        _unitsPools[unit.UnitData.KindOfUnit].Release(unit);
    }
}
