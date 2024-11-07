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
    //[SerializeField] private LocalPlayerUI _playerUI;
    
    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;

    private DamageSystem _damageSystem;
    public DamageSystem DamageSystem => _damageSystem;
    private void Awake()
    {
        _placementManager.SetGameManager(this);

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
        _gameGrid = new GameGrid(GridWidth, GridHeight, GridCellSize, CellTickRate, this);
    }

    private void Start()
    {
        Debug.Log(_playerController.Count);
    }
    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float halfSize = GridCellSize / 2f;

        float posX = 0 - ((GridWidth * GridCellSize) + halfSize);
        for (int i = -GridWidth; i <= GridWidth + 1; i++)
        {
            float posZ = 0 - ((GridHeight * GridCellSize) + halfSize);
            for (int j = -GridHeight; j <= GridHeight + 1; j++)
            {
                posZ += GridCellSize;
                Gizmos.DrawWireCube((new Vector3(posX, 0, posZ)), Vector3.one * GridCellSize);
            }
            posX += GridCellSize;
        }
    }
}
