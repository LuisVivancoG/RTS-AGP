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
    [SerializeField] private GameObject _spawnerA; //TODO replace spawner with Army camps position and update GenerateUnits method to use that position
    [SerializeField] private GameObject _spawnerB;
    private List<CellUnit> _unitsListA = new();
    private List<CellUnit> _unitsListB = new();

    [SerializeField] private GameObject practiceDummy;

    //private Vector3 _movementRequest = Vector3.zero;

    private void Start()
    {
        
    }

    private void Update()
    {
        foreach (var unit in _unitsListA)
        {
            if (_gameManager.GameGrid.CellIdFromPosition(unit.transform.position) != _gameManager.GameGrid.CellIdFromPosition(practiceDummy.transform.position))
            {
                unit.MoveToTarget(practiceDummy.transform.position);
            }
        }

        foreach (var unit in _unitsListB)
        {
            /*CellUnit closestEnemy = _gameManager.GameGrid.FindClosestOtherFactionUnit(unit, 10);
            if (closestEnemy != null)
            {
                if (_gameManager.GameGrid.CellIdFromPosition(unit.transform.position) != _gameManager.GameGrid.CellIdFromPosition(closestEnemy.transform.position))
                {
                    unit.MoveToTarget(closestEnemy.gameObject.transform.position);
                }
                else
                {
                    Debug.Log("No enemy in range");
                }
            }*/
        }
    }

    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        GenerateUnits(1, _unitToSpawnA, _spawnerA.transform, ref _unitsListA);
        GenerateUnits(2, _unitToSpawnB, _spawnerB.transform, ref _unitsListB);
    }

    private void GenerateUnits(int faction, CellUnit data, Transform spawner, ref List<CellUnit> list)
    {
        int mapWidth = _gameManager.GameGrid.Width * _gameManager.GameGrid.CellSize;
        int mapHeight = _gameManager.GameGrid.Height * _gameManager.GameGrid.CellSize;

        for (int i = 0; i < _unitsSpawned; i++)
        {
            CellUnit cellUnit = Instantiate(data, spawner.transform.position, Quaternion.identity, spawner);

            cellUnit.Setup(faction, i, _gameManager.GameGrid);

            list.Add(cellUnit);
        }
    }

    /*public void LocTarget(CellUnit unitToMove, Vector3 target)
    {
        if (unitToMove != null && target != null)
        {
            //_movementRequest = target;
            Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(target);
            unitToMove.MoveToTarget(cellInGrid);
        }
    }

    void MoveTo(CellUnit unitToMove)
    {
        Vector3 cellInGrid = _gameManager.GameGrid.GetCellWorldCenter(practiceDummy.transform.position);
        unitToMove.MoveToTarget(cellInGrid);
    }*/

    /*private void OnDrawGizmos()
    {
        if (_gameManager == null || _gameManager.GameGrid == null)
        {
            return;
        }
        Gizmos.color = Color.cyan;
        int w = _gameManager.GameGrid.Width;
        int h = _gameManager.GameGrid.Height;
        int size = _gameManager.GameGrid.CellSize;
        float halfSize = size / 2f;

        float posX = 0 - ((w * size) + halfSize);
        for (int i = -w; i <= w + 1; i++)
        {
            float posZ = 0 - ((h * size) + halfSize);
            for (int j = -h; j <= h + 1; j++)
            {
                posZ += size;

                Gizmos.DrawWireCube((new Vector3(posX, 0, posZ)), Vector3.one * size);

                //Debug.Log($"{posX},{posZ}");
            }
            posX += size;
        }

        Gizmos.color = Color.gray;

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, 20000, GroundMask))
        {
            var pos = _gameManager.GameGrid.GetCellWorldCenter(hitInfo.point);

            Gizmos.DrawWireCube(pos, Vector3.one * _gameManager.GameGrid.CellSize);
        }
    }*/
}
