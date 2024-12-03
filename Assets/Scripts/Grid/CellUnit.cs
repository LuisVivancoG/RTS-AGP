using System.Collections.Generic;
using UnityEngine;

public class CellUnit : MonoBehaviour //this is where units states are defined and where queue of nodes to track are stored.
                                      //Between the list of states available are FindCloseEnemyBuilding, FindCloseEnemyUnit and MoveToTarget.
                                      //TODO consider adding selfdestruction state
{
    [SerializeField] private float _moveSpeed = 5f;
    //[SerializeField] private Animator _animController;

    //[SerializeField] private UnitsData _scriptedObjectData;
    //public UnitsData _unitData => _scriptedObjectData;


    private int _faction;
    public int Faction => _faction;

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    private Vector3 _moveTarget;
    private Vector3 _previousPosition;
    private GameGrid _grid;

    private int _i;
    private IList<Vector2> _path;

    public bool _canMove;

    private int _health = 20;

    private void Update()
    {
        if (_canMove)
        {
            OnMovement();
        }
    }

    public void ChangeHealth(int change)
    {
        _health += change;
        // todo death check
    }

    public void Setup(int faction, int unitCounter, GameGrid gameGrid)
    {
        _faction = faction;
        name = $"P{faction}_{name}_{unitCounter}";
        _grid = gameGrid;
        //UpdatePreviousPosition();
    }

    public void UpdatePreviousPosition()
    {
        _previousPosition = transform.position;
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    void OnMovement()
    {
        if (_path == null || _i < 0 || _i >= _path.Count)
        {
            //_canMove = false;
            return;
        }

        float speed = Time.deltaTime * _moveSpeed;
        var nextNode = _grid.GetCellPositionFromId(_path[_i]);
        var centerNode = _grid.GetCellWorldCenter(nextNode);

        Vector3 directionToTarget = nextNode - transform.position;
        if (Vector3.Distance(transform.position, nextNode) > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget, transform.up);
        }

        transform.position = Vector3.MoveTowards(transform.position, centerNode, speed);

        if (Vector3.Distance(transform.position, nextNode) < 0.1f)
        {
            _i--;
        }

        _grid.UpdateUnitCell(this, _previousPosition);

        if (_i < 0)
        {
            _canMove = false;
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        _path = new List<Vector2>();

        target = _grid.ClampToCellBounds(target);

        if (_grid.CellIdFromPosition(transform.position) == _grid.CellIdFromPosition(target))
        {
            //Debug.Log($"Faction {_faction}: Already at target."); //delete this
            _canMove = false;
            return; 
        }
        _path = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, target);

        if (_path == null || _path.Count == 0)
        {
            //Debug.LogWarning($"Faction {_faction}: No valid path to {target}");
            _canMove = false;
            return;
        }
        //Debug.Log($"Faction {_faction}: Moving along path with {_path.Count} nodes");
        _i = _path.Count - 1;
        _canMove = true;
    }
}
