using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid //GameGrid is in charge of drawing the grid and make operations to convert Unity vectors into grid generated.
                      //Use this only for things related to grid cells checks
{

    private int _width, _height, _cellSize;
    public int Width => _width;
    public int Height => _height;
    public int CellSize => _cellSize;

    private Dictionary<Vector2, GridCell> _grid = new();
    public int CellCount => _grid.Count;

    private float _cellTickTimer = 0f;
    private float _cellTickRate;

    private GameManager _gameManager;
    public GameManager Manager => _gameManager;

    private Pathfinder _pathfinder;
    public Pathfinder Pathfinder => _pathfinder;

    public GameGrid(int mapWidth, int mapHeight, int cellSize, float tickRate, GameManager gameManager)
    {
        _width = mapWidth;
        _height = mapHeight;
        _cellSize = cellSize;
        _cellTickRate = tickRate;
        _gameManager = gameManager;
        GenerateGrid();
        _pathfinder = new Pathfinder(this, _grid);
    }

    private void GenerateGrid()
    {
        int w = _width;
        int h = _height;
        int size = _cellSize;
        float halfSize = size / 2f;

        float posX = 0 - ((w * size) + halfSize);
        for (int i = -w; i <= w + 1; i++)
        {
            float posZ = 0 - ((h * size) + halfSize);
            for (int j = -h; j <= h + 1; j++)
            {
                posZ += size;

                var cellId = new Vector2Int(i, j);

                _grid.Add(cellId, new GridCell(this));
                //Debug.Log(cellId);
            }
            posX += size;
        }
        //Debug.Log(_grid.Count);
    }

    public Vector3 ClampToCellBounds(Vector3 posToClamp)
    {
        return new Vector3(
            Mathf.Clamp(posToClamp.x, _width * -_cellSize, _width * _cellSize),
            posToClamp.y,
            Mathf.Clamp(posToClamp.z, _height * -_cellSize, _height * _cellSize));
    }

    public void UpdateUnitCell(CellUnit unit, Vector3 previousPosition)
    {
        previousPosition = ClampToCellBounds(previousPosition);
        Vector3 currentPosition = ClampToCellBounds(unit.transform.position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        var cellId = new Vector2Int(cellX, cellZ);

        if (!_grid.ContainsKey(cellId))
        {
            //Debug.LogWarning($"Cell {cellId} not found. Adding new cell.");
            _grid.Add(cellId, new GridCell(this));
        }

        //Debug.Log($"Unit {unit.name} moving to cell {cellId} at position {currentPosition}");
        _grid[cellId].AddUnitToCell(unit);
        unit.UpdatePreviousPosition();
    }

    public Vector3 GetCellWorldCenter(Vector3 location)
    {
        location = ClampToCellBounds(location);

        int cellX = (int)(location.x / _cellSize);
        int cellZ = (int)(location.z / _cellSize);

        float halfSize = _cellSize / 2f;

        float posX = ((cellX) * _cellSize);
        float posZ = ((cellZ) * _cellSize);

        if (location.x >= 0)
            posX += halfSize;
        else
            posX -= halfSize;

        if (location.z >= 0)
            posZ += halfSize;
        else
            posZ -= halfSize;

        return new Vector3(posX, location.y, posZ);
    }

    /*public CellUnit FindClosestOtherFactionUnit(CellUnit unitSearching)
    {
        Vector3 currentPosition = ClampToCellBounds(unitSearching.transform.position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        var cellId = new Vector2Int(cellX, cellZ);

        // validate this cell has even been registered
        if (!_grid.ContainsKey(cellId))
        {
            _grid.Add(cellId, new GridCell(this));
            return null;
        }

        CellUnit closestEnemy = null;

        // check again all other units in our grid cell
        var otherUnitsList = _grid[cellId].GetOtherFactionUnits(unitSearching.Faction);

        float smallestDistance = Mathf.Infinity;

        foreach (var otherUnit in otherUnitsList)
        {
            float distSqr = (otherUnit.transform.position - unitSearching.transform.position).sqrMagnitude;

            if (distSqr < smallestDistance)
            {
                smallestDistance = distSqr;
                closestEnemy = otherUnit;
            }
        }

        return closestEnemy;
        // we could also check the surrounding grid cells
    }*/
    
    public CellUnit FindClosestOtherFactionUnit(CellUnit unitSearching, int rangeToCheck)  //Checks a list of cells around its position depending on unit/building range.
                                                                                           //If range is 0, only checks at same cell it is, 1 or bigger is the amount of cells around the origin 
    {
        Vector3 currentPosition = ClampToCellBounds(unitSearching.transform.position); //clamp unit current pos to cell bounds
        var cellsInRange = GetCellsAroundPosition(currentPosition, rangeToCheck); //Gets list<Vector2> of cells around unit based on unitRange

        if (cellsInRange == null || cellsInRange.Count == 0)
        {
            //Debug.LogWarning($"No cells found in range {rangeToCheck} for unit {unitSearching.name}");
            return null;
        }

        CellUnit closestEnemy = null;
        float smallestDistance = Mathf.Infinity;

        foreach (var cell in cellsInRange)
        {
            var otherUnitsList = cell.GetOtherFactionUnits(unitSearching.Faction); //Tries to get any unit from different faction within list<Vector2> cells

            foreach (var otherUnit in otherUnitsList)
            {
                float distSqr = (otherUnit.transform.position - unitSearching.transform.position).sqrMagnitude;

                Vector3 directionEnemy = (otherUnit.transform.position - unitSearching.transform.position).normalized;
                float angle = Vector3.Angle(unitSearching.transform.forward, directionEnemy);

                //Debug.Log($"Checking unit {otherUnit.name}, distance: {Mathf.Sqrt(distSqr)}, angle: {angle}");


                if (distSqr < smallestDistance) //checks if multiple units found, pick the one in less distance
                {
                    smallestDistance = distSqr;
                    closestEnemy = otherUnit;
                }
            }
        }

        if (closestEnemy != null)
        {
            //Debug.Log($"Closest enemy to {unitSearching.name} is {closestEnemy.name} at distance {Mathf.Sqrt(smallestDistance)}");
        }
        else
        {
            //Debug.Log($"No enemies found in range {rangeToCheck} for unit {unitSearching.name}");
        }

        return closestEnemy; //Returns the enemy unit that is closest. Null if there is no enemy unit
    }

    public Vector2 CellIdFromPosition(Vector3 position)
    {
        Vector3 currentPosition = ClampToCellBounds(position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        return new Vector2Int(cellX, cellZ);
    }

    public Vector3 GetCellPositionFromId(Vector2 cellId)
    {
        int cellX = (int)(cellId.x * _cellSize);
        int cellZ = (int)(cellId.y * _cellSize);

        return new Vector3(cellX, 0, cellZ);
    }

    public PlacedBuildingBase FindClosestEnemySpawnBuilding(CellUnit unitSearching)
    {
        PlacedBuildingBase closestEnemySpawnBuilding = null;

        float smallestDistance = Mathf.Infinity;

        // todo, optimize to check a list of buildings instead of the entire grid
        foreach (var cell in _grid)
        {
            var building = cell.Value.BuildingInCell;
            if (building != null &&
                building is TownHall &&
                building.GetFaction() != unitSearching.Faction)
            {
                float distSqr = (building.transform.position - unitSearching.transform.position).sqrMagnitude;

                if (distSqr < smallestDistance)
                {
                    smallestDistance = distSqr;
                    closestEnemySpawnBuilding = building;
                }
            }
        }
        return closestEnemySpawnBuilding;
    }

    /*public void OnUpdate()
    {
        _cellTickTimer += Time.deltaTime;

        if (_cellTickTimer >= _cellTickRate)
        {
            _cellTickTimer = 0f;
            TickAllGrids();
        }
    }

    private void TickAllGrids()
    {
        foreach (GridCell grid in _grid.Values)
        {
            grid.OnTick();
        }
    }*/


    private GridCell GetCellAtPosition(Vector3 position)
    {
        Vector3 currentPosition = ClampToCellBounds(position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        var cellId = new Vector2Int(cellX, cellZ);

        // validate this cell has even been registered
        if (!_grid.ContainsKey(cellId))
        {
            _grid.Add(cellId, new GridCell(this));
        }
        return _grid[cellId];
    }

    public List<GridCell> GetCellsAroundPosition(Vector3 position, int range)
    {
        List<GridCell> cells = new();

        Vector3 centeredPosition = ClampToCellBounds(position);

        int cellX = (int)(centeredPosition.x / _cellSize);
        int cellZ = (int)(centeredPosition.z / _cellSize);

        int minX = Mathf.Clamp(cellX - range, -Width, Width);
        int maxX = Mathf.Clamp(cellX + range, minX, Width);
        int minZ = Mathf.Clamp(cellZ - range, -Height, Height);
        int maxZ = Mathf.Clamp(cellZ + range, minZ, Height);

        //Debug.Log($"Getting cells between: {minX} to {maxX} horizontal and {minZ} to {maxZ} vertical");
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                var cellId = new Vector2(x, z);

                if (!_grid.ContainsKey(cellId))
                {
                    _grid.Add(cellId, new GridCell(this));
                }

                if (!cells.Contains(_grid[cellId]))
                {
                    cells.Add(GetCellAtPosition(cellId));
                }
            }
        }

        return cells;
    }

    public Vector3 FindClosestEnemySpawnBuilding()
    {
        // todo get the list of all enemy spawn building from each factions BuildingManager 

        // todo get the edge of the building (the cell it occupies)
        return Vector3.zero;
    }

    public LinkedList<PathNode> FindPath(Vector3 StartPosition, Vector3 EndPosition)
    {
        // todo loop through the nodes along the path and try to find the least cost path to our objective

        // todo if our objective is blocked, find the least cost path and start destroying buildings in our way!

        // todo not here - When we are moving along the path, if we encounter enemy units, fight them first!
        return new LinkedList<PathNode>();
    }
}