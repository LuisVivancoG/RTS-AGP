using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int GridWidth = 10;
    [SerializeField] private int GridHeight = 10;
    [SerializeField] private int GridCellSize = 10;
    [SerializeField] private float CellTickRate;

    [SerializeField] private GameObject _playersGrp;
    [SerializeField] private int _playerCount = 1;
    [SerializeField] private BuildingPlacementManager _placementManager;
    [SerializeField] private UnitsManager _unitsManager;
    private SortedList<int, Player> _playerController = new();

    [Header ("Tutorial")]
    [SerializeField] private string _mainMenuScene;
    [SerializeField] private float _transitionDelay;
    [SerializeField] private bool _tutorialCompleted = false;
    [SerializeField] private bool _playerBaseDestroyed = false;
    [SerializeField] private bool _botBaseDestroyed = false;
    private WaitForSeconds _waitBeforeTransition;

    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;

    public enum Contestants
    {
        player = 0,
        pc = 1,
    }

    /*private DamageSystem _damageSystem;
    public DamageSystem DamageSystem => _damageSystem;*/

    //[SerializeField] private N_UnitsManager _NUnitsManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

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
    }

    private void Start()
    {
        _placementManager.SetGameManager(this);
        _unitsManager.SetGameManager(this);
        //_NUnitsManager.SetGameManager(this);

        _waitBeforeTransition = new WaitForSeconds(_transitionDelay);
        StartCoroutine(GameLoop());
    }

    public void BaseDestroyed(Contestants champ)
    {
        switch (champ)
        {
            case Contestants.player:
                _playerBaseDestroyed = true;
                break;

            case Contestants.pc:
                _botBaseDestroyed = true;
                break;
        }
    }

    IEnumerator GameLoop()
    {
        yield return StartCoroutine(TutorialGameplay());
        Debug.LogWarning("Starting the gameplay");
        yield return StartCoroutine(GameStarted());
        Debug.LogWarning("Showing results");
        yield return StartCoroutine(ShowResults());
        Debug.LogWarning("Ready for transition");
        SceneManager.LoadScene(_mainMenuScene);
    }

    IEnumerator TutorialGameplay()
    {

        while (!_tutorialCompleted)
        {
            yield return null;
        }
    }

    IEnumerator GameStarted()
    {

        while (!_playerBaseDestroyed && !_botBaseDestroyed)
        {
            yield return null;
        }
    }

    IEnumerator ShowResults()
    {
        yield return _waitBeforeTransition;
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
