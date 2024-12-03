using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int GridWidth = 10;
    [SerializeField] private int GridHeight = 10;
    [SerializeField] private int GridCellSize = 10;
    [SerializeField] private float CellTickRate;

    [SerializeField] private GameObject _playersGrp;
    [SerializeField] private int _playerCount = 1;
    private SortedList<int, Player> _playerController = new();
    [SerializeField] private BuildingPlacementManager _placementManager;

    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;

    /*private DamageSystem _damageSystem;
    public DamageSystem DamageSystem => _damageSystem;*/

    [SerializeField] private UnitsManager _unitsManager;
    private void Awake()
    {
        _gameGrid = new GameGrid(GridWidth, GridHeight, GridCellSize, CellTickRate, this);

        for (int i=0; i<_playerCount; i++)
        {
            var player = new Player(i, i, this);
            _playerController.Add(i, player);
            if (i == 0)
            {
                _placementManager.SetLocalBuildingManager(player.BuildingManager);
                //_playerUI.SusbcribeToPlayerUpdates(player);
            }
        }
        //Debug.Log(GameGrid.Pathfinder._walkablePositions.Count);
        //Debug.Log(GameGrid.Pathfinder._walkablePositions.Count);
    }

    private void Start()
    {
        _placementManager.SetGameManager(this);
        _unitsManager.SetGameManager(this);
    }

    private void OnDrawGizmos()
    {
        if (_gameGrid != null)
        {
            Gizmos.color = Color.gray;
            float halfSize = GridCellSize / 2f;

            float posX = 0 - ((GridWidth * GridCellSize) + halfSize);
            for (int i = -GridWidth; i <= GridWidth + 1; i++)
            {
                float posZ = 0 - ((GridHeight * GridCellSize) + halfSize);
                for (int j = -GridHeight; j <= GridHeight + 1; j++)
                {
                    posZ += GridCellSize;
                    Gizmos.DrawWireCube((new Vector3(posX, halfSize, posZ)), Vector3.one * GridCellSize);
                }
                posX += GridCellSize;
            }
        }
    }
}
