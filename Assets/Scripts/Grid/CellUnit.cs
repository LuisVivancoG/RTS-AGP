using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class CellUnit : MonoBehaviour //this is where units states are defined and where queue of nodes to track are stored.
                                      //Between the list of states available are FindCloseEnemyBuilding, FindCloseEnemyUnit and MoveToTarget.
                                      //TODO consider adding selfdestruction state
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Animator _animController;

    [SerializeField] private UnitsData _scriptedObjectData;
    public UnitsData _unitData => _scriptedObjectData;


    private int _faction;
    public int Faction => _faction;

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    private Vector3 _moveTarget;
    private Vector3 _previousPosition;
    private GameGrid _grid;

    public IList<Vector2> _pathToFollow { get; private set; }
    private int i;
    //public Vector3 _currentDestination { get; private set; }

    public bool _canMove;

    private int _health = 20;

    private void Update() //Ask if there is another way and more efficient to make the movement without requiring Update method.
                          //Why MoveToEnemy and RandomMovement work?
    {
        ////Old ver, using QUEUE
        //if (_pathToFollow.Count > 0)
        //{ 
        //    //_animController.SetFloat("Velocity", 1);
        //    Vector3 targetPosition = _pathToFollow.Peek();
        //    Vector3 direction = (targetPosition - transform.position).normalized;
        //    transform.position += direction * _moveSpeed * Time.deltaTime;
        //    transform.rotation = Quaternion.LookRotation(direction);

        //    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        //    {
        //        _pathToFollow.Clear();
        //        MoveToTarget(_currentDestination);
        //    }
        //}
        //else
        //{
        //    _animController.SetFloat("Velocity", 0);
        //}
        if (_canMove)
        {
            Move();
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
        //_pathToFollow = new IList<Vector2>();
        GetNewRandomTarget();
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    public void RandomMove() //delete later
    {
        //_animController.SetFloat("Velocity", 1);
        // lazy translate move instead of physics
        transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed);

        _grid.UpdateUnitCell(this, _previousPosition);

        _previousPosition = transform.position;

        if ((transform.position - _moveTarget).magnitude < 10f)
        {
            GetNewRandomTarget();
        }
    }

    private void GetNewRandomTarget()
    {
        int mapWidth = _grid.Width * _grid.CellSize;
        int mapHeight = _grid.Height * _grid.CellSize;

        _moveTarget = new Vector3(UnityEngine.Random.Range(-mapWidth, mapWidth), transform.position.y, UnityEngine.Random.Range(-mapHeight, mapHeight));

        transform.rotation = Quaternion.LookRotation(_moveTarget - transform.position);
    }

    public void Move()
    {
        float step = Time.deltaTime * _moveSpeed;
        var target = _grid.GetCellPositionFromId(_pathToFollow[i]);
        //_moveTarget = _grid.GetCellWorldCenter(target);
        transform.rotation = Quaternion.LookRotation(target - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        _previousPosition = transform.position;
        _grid.UpdateUnitCell(this, _previousPosition);

        if (transform.position == target && i >= 0)
            i--;
        //Instantiate(gameObject, target, Quaternion.identity);
        Debug.Log(target);

        if (i < 0)
        {
            _canMove = false;
        }
    }
    public void MoveToTarget(Vector3 target)
    {
        if (transform.position != target)
        {
            //endPos = target;

            _pathToFollow = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, target);
            i = _pathToFollow.Count - 1;
        }
    }

    /*public void MoveToTarget(Vector3 target)
    {
        //_isIdle = false;
        //_currentDestination = target;
        _previousPosition = transform.position;

        if (_grid.CellIdFromPosition(transform.position) == _grid.CellIdFromPosition(target))
        {
            transform.rotation = Quaternion.LookRotation(target - transform.position);
        }
        else
        {
            _pathToFollow = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, target);

            for(int x=0; x < _pathToFollow.Count; x++)
            {
                transform.Translate(_pathToFollow[x] * Time.deltaTime * _moveSpeed);

                _grid.UpdateUnitCell(this, _previousPosition);

                _previousPosition = transform.position;
            }
            //_isIdle = true;
            //_currentDestination = transform.position;
        }

        //if (_grid.CellIdFromPosition(transform.position) == _grid.CellIdFromPosition(target))
        //{
        //    transform.rotation = Quaternion.LookRotation(target - transform.position);
        //}
        //else
        //{
        //    _pathToFollow.Clear();
        //    var path = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, target);
        //    Debug.Log("Pathfinder check");
        //    if (path == null || path.Count == 0)
        //    {
        //        Debug.LogWarning("Failed to find path!");

        //        return;
        //    }

        //    foreach (var node in path)
        //    {
        //        Vector3 nodePosition = _grid.GetCellPositionFromId(node);
        //        _pathToFollow.Enqueue(nodePosition);
        //    }

        //    //Debug.Log($"Path to follow contains {_pathToFollow.Count} waypoints.");
        //}
    }*/

    public void MoveToEnemy(CellUnit otherUnit)
    {
        float minDistance = math.distance(_grid.CellIdFromPosition(transform.position), _grid.CellIdFromPosition(otherUnit.transform.position));
        // in same cell
        if (minDistance <= _unitData.AttackRange)
        {
            transform.rotation = Quaternion.LookRotation(otherUnit.transform.position - transform.position);
        }
        else
        {
            // else find path

            var path = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, otherUnit.transform.position);

            if (path == null)
            {
                Debug.LogWarning("Failed to find path!");
                return;
            }
            var firstNode = path.FirstOrDefault();
            if (firstNode == null)
            {
                Debug.LogWarning("Failed to find node!");
                return;
            }

            transform.rotation = Quaternion.LookRotation(_grid.GetCellPositionFromId(firstNode));
        }

        transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed);

        _grid.UpdateUnitCell(this, _previousPosition);

        _previousPosition = transform.position;
    }

    public void MoveToEnemy(PlacedBuildingBase building)
    {
        float minDistance = math.distance(_grid.CellIdFromPosition(transform.position), _grid.CellIdFromPosition(building.transform.position));
        // in same cell
        if (minDistance <= _unitData.AttackRange)
        {
            transform.rotation = Quaternion.LookRotation(building.transform.position - transform.position);
        }
        else
        {
            var path = _grid.Pathfinder.FindShortestPath(Pathfinder.PathfindingType.AStarManhattan, transform.position, building.transform.position);

            if (path == null)
            {
                Debug.LogWarning("Failed to find path!");
                return;
            }
            var firstNode = path.FirstOrDefault();
            if (firstNode == null)
            {
                Debug.LogWarning("Failed to find node!");
                return;
            }

            transform.rotation = Quaternion.LookRotation(_grid.GetCellPositionFromId(firstNode));
        }

        transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed);

        _grid.UpdateUnitCell(this, _previousPosition);

        _previousPosition = transform.position;
    }

    public void OnDrawGizmos()
    {

        if (_pathToFollow == null)
        {
            return;
        }

        _grid.Pathfinder.DrawPositions();

        for (int x = _pathToFollow.Count; x > 0; x--)
        {
            Gizmos.color = Color.yellow;
            int size = _grid.CellSize;

            var target = _grid.GetCellPositionFromId(_pathToFollow[x]);
            _moveTarget = _grid.GetCellWorldCenter(target);

            Gizmos.DrawWireCube(_moveTarget, Vector3.one * size);
        }
    }
}
