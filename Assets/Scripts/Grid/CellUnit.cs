using System;
using System.Collections.Generic;
using UnityEngine;

public class CellUnit : MonoBehaviour //this is where units states are defined and where queue of nodes to track are stored.
                                      //Between the list of states available are FindCloseEnemyBuilding, FindCloseEnemyUnit and MoveToTarget.
                                      //TODO consider adding selfdestruction state
{
    private GameGrid _grid;
    private float _moveSpeed;
    private bool _canMove = false;
    private int _faction;
    public int Faction => _faction;

    private IList<Vector2> _path;
    private int _index;

    public Vector3 _currentDestination {  get; private set; }

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    private Transform _enemyUnit = null;
    public Transform EnemyUnit => _enemyUnit;

    private Action _onMovement;
    private Action _onIdle;

    private void Update()
    {
        if (_canMove)
        {
            OnMovement();
            _onMovement?.Invoke();
        }
        else
        {
            _onIdle?.Invoke();
        }    

        if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log($"Unit faction {Faction} currently at {_grid.CellIdFromPosition(this.transform.position)}");
        }
    }

    public void SetData(UnitsBase unitPrefab, Action movement, Action idle)
    {
        _moveSpeed = unitPrefab.UnitData.MovementSpeed;
        _onMovement = movement;
        _onIdle = idle;
    }

    public void Setup(int faction/*, int unitCounter*/, GameGrid gameGrid)
    {
        _faction = faction;
        //name = $"P{faction}_{name}";
        _grid = gameGrid;
        _currentDestination = transform.position;
    }

    public void SetEnemyTarget(CellUnit enemyFound)
    {
        _enemyUnit = enemyFound.transform;
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    void OnMovement()
    {
        /*if (_path == null || _index < 0 || _index >= _path.Count)
        {
            _canMove = false;
            _onIdle?.Invoke();
            _currentCommand = Vector3.zero;
            return;
        }*/

        float speed = Time.deltaTime * _moveSpeed;
        var nextNode = _grid.GetCellPositionFromId(_path[_index]);
        var centerNode = _grid.GetCellWorldCenter(nextNode);

        Vector3 directionToTarget = centerNode - transform.position;
        if (Vector3.Distance(transform.position, centerNode) > _grid.CellSize)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget, transform.up);
        }

        transform.position = Vector3.MoveTowards(transform.position, centerNode, speed);

        _grid.UpdateUnitCell(this);

        if (Vector3.Distance(transform.position, centerNode) < 0.1f)
        {
            _index--;
        }

        if (_index < 0)
        {
            _canMove = false;
            //_onIdle?.Invoke();
            //_currentCommand = Vector3.zero;
        }

        
    }

    public void SetNewTarget(Vector3 target)
    {
        _currentDestination = target;
        _path = new List<Vector2>();
        ValidatePathFinder();
    }

    void ValidatePathFinder()
    {
        var targetCenteredCell = _grid.GetCellWorldCenter(_currentDestination);

        if (_grid.CellIdFromPosition(transform.position) == _grid.CellIdFromPosition(targetCenteredCell))
        {
            Debug.Log($"Unit faction {_faction} reached the target.");
            _canMove = false;
            _path.Clear();
            return;
        }

        if (_grid.CellIdFromPosition(transform.position) != _grid.CellIdFromPosition(targetCenteredCell))
        {
            var newPath = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, targetCenteredCell);
            if (newPath == null || newPath.Count == 0)
            {
                Debug.LogWarning($"Faction {_faction}: No valid path to {targetCenteredCell}");
                _canMove = false;
                return;
            }
            else
            {
                _path = newPath;
                _index = newPath.Count - 1;
                _canMove = true;
            }
        }
    }
}
