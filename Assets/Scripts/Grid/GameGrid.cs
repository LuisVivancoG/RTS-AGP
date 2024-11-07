using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid
{

    private int _width, _height, _cellSize;
    public int Width => _width;
    public int Height => _height;
    public int CellSize => _cellSize;

    private Dictionary<Vector3, GridCell> _grid = new();
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

                var cellId = new Vector3Int(i, 0, j);

                _grid.Add(cellId, new GridCell(this));
            }
            posX += size;
        }
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

        int oldCellX = (int)(previousPosition.x / _cellSize);
        int oldCellZ = (int)(previousPosition.z / _cellSize);

        Vector3 currentPosition = ClampToCellBounds(unit.transform.position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        //If it didn't change cell, we are done
        if (oldCellX == cellX && oldCellZ == cellZ)
        {
            return;
        }
        var cellId = new Vector3Int(cellX, 0, cellZ);

        if (!_grid.ContainsKey(cellId))
        {
            _grid.Add(cellId, new GridCell(this));
        }

        _grid[cellId].AddUnitToCell(unit);
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

    public CellUnit FindClosestOtherFactionUnit(CellUnit unitSearching)
    {
        Vector3 currentPosition = ClampToCellBounds(unitSearching.transform.position);

        int cellX = (int)(currentPosition.x / _cellSize);
        int cellZ = (int)(currentPosition.z / _cellSize);

        var cellId = new Vector3Int(cellX, 0, cellZ);

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
    }
}