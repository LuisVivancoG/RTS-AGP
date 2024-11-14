using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour //This manager gives orders to the units defined by UnitselectionSystem.
                                          //Between its functions are: generate units on demand, establishes priority of base state machine and
                                          //override FSM by assigning new pathfinding or forcing to stay still.
{
    public GameManager _gameManager { get; private set;}

    [SerializeField] private int _unitsSpawned;
    [SerializeField] private CellUnit _unitToSpawnA;
    [SerializeField] private CellUnit _unitToSpawnB;
    [SerializeField] private GameObject _spawner; //TODO replace spawner with Army camps position and update GenerateUnits method to use that position
    [SerializeField] private List<CellUnit> _unitsListA = new();
    [SerializeField] private List<CellUnit> _unitsListB = new();

    private void Update()
    {
        if (_unitsListB.Count > 0)
        {
            foreach (CellUnit unit in _unitsListB)
            {
                if (BuildingCheck(unit)) continue;
                if (EnemyCheck(unit)) continue;
                unit.RandomMove();
            }
        }

        //if (_unitsListA.Count > 0)
        //{
        //    foreach (CellUnit unit in _unitsListA)
        //    {
        //        if (BuildingCheck(unit)) continue;
        //        if (EnemyCheck(unit)) continue;
        //        // if none, patrol randomly
        //        unit.RandomMove();
        //    }
        //}
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        GenerateUnits(1, _unitToSpawnA, _spawner, ref _unitsListA);
        GenerateUnits(2, _unitToSpawnB, _spawner, ref _unitsListB);
    }

    private void GenerateUnits(int faction, CellUnit data, GameObject spawner, ref List<CellUnit> list)
    {
        if(faction != 1)
        {
            int mapWidth = _gameManager.GameGrid.Width * _gameManager.GameGrid.CellSize;
            int mapHeight = _gameManager.GameGrid.Height * _gameManager.GameGrid.CellSize;

            for (int i = 0; i < _unitsSpawned; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-mapWidth, mapWidth), 0f, Random.Range(-mapHeight, mapHeight));

                CellUnit unitInstance = Instantiate<CellUnit>(data, randomPos, Quaternion.identity, spawner.transform);

                unitInstance.Setup(faction, i, _gameManager.GameGrid);

                list.Add(unitInstance);
            }
        }
        else
        {
            var pos = _gameManager.GameGrid.GetCellWorldCenter(spawner.transform.position);

            for (int i = 0; i < _unitsSpawned; i++)
            {
                CellUnit unitInstance = Instantiate<CellUnit>(data, pos, Quaternion.identity, spawner.transform);

                unitInstance.Setup(faction, i, _gameManager.GameGrid);

                list.Add(unitInstance);
            }
        }
    }

    private bool EnemyCheck(CellUnit unit)
    {
        // Find enemy within vision range (currently, same cell only)
        CellUnit closestEnemy = _gameManager.GameGrid.FindClosestOtherFactionUnit(unit);

        if (closestEnemy != null)
        {
            unit.MoveToEnemy(closestEnemy);
            return true;
        }
        return false;
    }
    private bool BuildingCheck(CellUnit unit)
    {
        // Find closest enemy spawn building
        PlacedBuildingBase closestSpawnBuilding = _gameManager.GameGrid.FindClosestEnemySpawnBuilding(unit);

        if (closestSpawnBuilding != null)
        {
            unit.MoveToEnemy(closestSpawnBuilding);
            return true;
        }
        return false;
    }
    public void LocTarget(CellUnit unitToMove, Vector3 target)
    {
        Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(target);
        unitToMove.MoveToTarget(cellInGrid);
    }
}
