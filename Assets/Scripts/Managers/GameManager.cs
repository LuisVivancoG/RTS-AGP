using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int GridWidth = 10;
    [SerializeField] private int GridHeight = 10;
    [SerializeField] private int GridCellSize = 10;

    [SerializeField] private GameObject _playersGrp;
    [SerializeField] private int _playerCount = 1;
    private SortedList<int, Player> _playerController = new();
    [SerializeField] private BuildingPlacementManager _placementManager;
    //[SerializeField] private LocalPlayerUI _playerUI;
    
    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;
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
        _gameGrid = new GameGrid(GridWidth, GridHeight, GridCellSize, this);
    }

    private void Start()
    {
        Debug.Log(_playerController.Count);
    }
    private void Update()
    {
        
    }
}
