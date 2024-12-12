using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class N_CellUnit : MonoBehaviour
{
    [SerializeField] private int _faction;
    private GameGrid _grid;

    private float _moveSpeed = 50f;
    private bool _canMove = false;

    private IList<Vector2> _path;
    private int _index;

    private Vector3 _currentDestination;

    private Action _onMovement;
    private Action _onIdle;

    public void SetUp(GameGrid grid, int faction)
    {
        _grid = grid;
        _faction = faction;
    }

    void Update()
    {
        if (_canMove)
        {
            OnMovement();
            //_onMovement?.Invoke();
        }
        /*else
        {
            //_onIdle?.Invoke();
        }*/
    }

    private void OnMovement()
    {
        /*if (_path == null || _i < 0 || _i >= _path.Count)
        {
            _canMove = false;
            _onIdle?.Invoke();
            _currentCommand = Vector3.zero;
            return;
        }*/

        float speed = Time.deltaTime * _moveSpeed;
        var nextNode = _grid.GetCellPositionFromId(_path[_index]);
        var centerNode = _grid.GetCellWorldCenter(nextNode);

        Vector3 directionToTarget = nextNode - transform.position;
        if (Vector3.Distance(transform.position, nextNode) > _grid.CellSize)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget, transform.up);
        }

        transform.position = Vector3.MoveTowards(transform.position, centerNode, speed);

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

        //_grid.UpdateUnitCell(this, _previousPosition);
        ValidatePathfinder();
    }
    public void SetNewTarget(Vector3 target)
    {
        _currentDestination = target;
        _path = new List<Vector2>();
        ValidatePathfinder();
    }


    private void ValidatePathfinder()
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
                //_canMove = false;
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
