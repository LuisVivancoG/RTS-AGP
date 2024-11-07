using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CellUnit : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private int _faction;
    public int Faction => _faction;

    private GridCell _currentCell = null;
    public GridCell CurrentCell => _currentCell;

    private Vector3 _moveTarget;
    private Vector3 _previousPosition;
    private GameGrid _grid;

    private int _health = 20;

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
        GetNewRandomTarget();
    }

    public void SetCell(GridCell gridCell)
    {
        _currentCell = gridCell;
    }

    public void RandomMove()
    {
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

        _moveTarget = new Vector3(Random.Range(-mapWidth, mapWidth), transform.position.y, Random.Range(-mapHeight, mapHeight));

        transform.rotation = Quaternion.LookRotation(_moveTarget - transform.position);
    }
}
