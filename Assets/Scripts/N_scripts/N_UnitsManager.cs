using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_UnitsManager : MonoBehaviour
{
    public GameManager _gameManager { get; private set; }

    [SerializeField] private int _unitsSpawned;

    //[SerializeField] private AllUnitsData _allUnitsData;

    [SerializeField] private N_CellUnit _unitToSpawnA;
    [SerializeField] private N_CellUnit _unitToSpawnB;
    [SerializeField] private GameObject _spawnerA; //TODO replace spawner with Army camps position and update GenerateUnits method to use that position
    [SerializeField] private GameObject _spawnerB;
    private List<N_CellUnit> _unitsListA = new();
    private List<N_CellUnit> _unitsListB = new();

    [SerializeField] private Transform _targetUnitsA;
    [SerializeField] private Transform _targetUnitsB;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Requesting Units A movement");
            foreach (var unit in _unitsListA)
            {
                unit.SetNewTarget(_targetUnitsA.position);
                /*if (MovementOrder(unit, unit.CellUnit._currentCommand)) continue;
                if (EnemyCheck(unit)) continue;
                if (BuildingCheck(unit)) continue;*/
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Requesting Units B movement");
            foreach (var unit in _unitsListB)
            {
                unit.SetNewTarget(_targetUnitsB.position);
                /*if (MovementOrder(unit, unit.CellUnit._currentCommand)) continue;
                if (EnemyCheck(unit)) continue;
                if (BuildingCheck(unit)) continue;*/
            }
        }
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        GenerateUnits(1, _unitToSpawnA, _spawnerA.transform, ref _unitsListA);
        GenerateUnits(2, _unitToSpawnB, _spawnerB.transform, ref _unitsListB);
    }

    private void GenerateUnits(int faction, N_CellUnit data, Transform spawner, ref List<N_CellUnit> list)
    {
        //int mapWidth = _gameManager.GameGrid.Width * _gameManager.GameGrid.CellSize;
        //int mapHeight = _gameManager.GameGrid.Height * _gameManager.GameGrid.CellSize;
        Debug.Log($"Generating units from faction {faction}");

        for (int i = 0; i < _unitsSpawned; i++)
        {
            var cellUnit = Instantiate(data, spawner.transform.position, Quaternion.identity, spawner);

            cellUnit.SetUp(_gameManager.GameGrid, faction);

            list.Add(cellUnit);
        }
    }

    /*public void LocTarget(N_CellUnit unitToMove, Vector3 target)
    {
        if (unitToMove != null && target != null)
        {
            Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(target);
            unitToMove.SetNewTarget(cellInGrid);
        }
    }

    private bool EnemyCheck(UnitsBase unit)
    {
        CellUnit closestEnemy = _gameManager.GameGrid.FindClosestOtherFactionUnit(unit.CellUnit, unit.UnitData.MaxDetectionRange);

        if (closestEnemy != null)
        {
            unit.CellUnit.MoveToTarget(closestEnemy.transform.position);
            Debug.Log($"Unit: {unit} encounterd enemy: {closestEnemy} moving towards it");
            return true;
        }
        return false;
    }
    private bool BuildingCheck(UnitsBase unit)
    {
        PlacedBuildingBase closestFoeBuilding = _gameManager.GameGrid.FindClosestEnemyBuilding(unit.CellUnit, unit.UnitData.MaxDetectionRange);

        if (closestFoeBuilding != null)
        {
            unit.CellUnit.MoveToTarget(closestFoeBuilding.transform.position);
            Debug.Log($"Unit: {unit} found building: {closestFoeBuilding} moving towards it");
            return true;
        }
        return false;
    }

    private bool MovementOrder(UnitsBase unit, Vector3 target)
    {
        if (Vector3.Distance(unit.transform.position, target) > _gameManager.GameGrid.CellSize)
        {
            Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(target);
            unit.CellUnit.MoveToTarget(cellInGrid);
            Debug.Log($"Unit: {unit} following requested location {target}");
            return true;
        }
        else
        {
            //unit.CellUnit._currentCommand = Vector3.zero;
            return false;
        }
    }*/
}
