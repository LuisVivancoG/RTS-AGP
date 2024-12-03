using System.Collections.Generic;
using UnityEngine;
using System;
using Pool;
using System.Linq;

/// <summary>
/// This is the controller for placing buildings.
/// It receives input via the <see cref="BuildingPlacementUI"/>
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    [Header("ObjectPooling")]
    [SerializeField] private AllBuildingsData _allBuildingData;
    internal AllBuildingsData _runTimeBuildingsData => _allBuildingData;
    private Dictionary<string, GameObject> _ghostsObjectsPool = new();
    private Dictionary<BuildingType, BuildingPool> _buildingsPools;

    [Header("Placement buildings")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _buildingsMask;
    private BuildingData _buildingToPlace = null;
    private GameObject _placementGhost = null;
    private bool _allowPlacement = true;

    [SerializeField] private ParticleSystem _placedParticles;
    [SerializeField] private GameObject _buildingsGrp;
    [SerializeField] private GameObject _ghostsGrp;

    [SerializeField] private Material _canPlaceMaterial;
    [SerializeField] private Material _cannotPlaceMaterial;

    [Header("Player settings")]
    [SerializeField] private GameManager _gameManager;
    private PlayerBuildingsManager _localPlayerBuildingManager = null;

    [SerializeField] private UIManager _uiManager;


    private void Start()
    {
        InitializePool();
    }

    private void Update()
    {
        if (!_allowPlacement)
        {
            return;
        }
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Cast a ray from camera that updates with mouse position
        if (_buildingToPlace == null) //if there is no building to place then check for left click
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hitInfo, 10000, _buildingsMask)) //if the position was on a building placed then retrieve info
                {
                    var buildingClicked = hitInfo.transform.GetComponentInParent<PlacedBuildingBase>();
                    Debug.Log(buildingClicked);
                    BuildingOptions(buildingClicked);
                }
            }
            return;
        }

        if (Physics.Raycast(ray, out hitInfo, 10000, _groundMask)) //if the raycast touches groundMask
        {
            if (_placementGhost != null) //if ghost is assigned then disable ghost
            {
                _placementGhost.SetActive(false); 
            }

            if (_ghostsObjectsPool.TryGetValue(_buildingToPlace.BuildingGhostPrefab.name, out var existingGhost)) //look on the ghost pool for buildingGhostPrefab
            {
                _placementGhost = existingGhost;
                _placementGhost.SetActive(true);
            }

            else //if buildingGhostPrefab was not on pool then instantiate a new one and add it to the pool
            {
                _placementGhost = Instantiate(_buildingToPlace.BuildingGhostPrefab, transform);
                _placementGhost.transform.parent = _ghostsGrp.transform;
                _ghostsObjectsPool.Add(_buildingToPlace.BuildingGhostPrefab.name, _placementGhost);
            }

            _placementGhost.transform.position = _gameManager.GameGrid.GetCellWorldCenter(hitInfo.point);

            var pos = _gameManager.GameGrid.GetCellWorldCenter(hitInfo.point);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding(pos);
                //var cellIdBlocked = _gameManager.GameGrid.CellIdFromPosition(pos);

                _buildingToPlace = null;
                _placementGhost.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1))
            {
                _buildingToPlace = null;
                _placementGhost.SetActive(false);
            }
        }
    }

    internal void InitializePool()
    {

        _buildingsPools = new Dictionary<BuildingType, BuildingPool>(); //Creates a new _buildingsPools
        foreach (BuildingType type in Enum.GetValues(typeof(BuildingType))) //Adds a pool for each BuildingType in the _buildingsPools
        {
            _buildingsPools.Add(type, new BuildingPool(() => CreatePoolBuildingType(type), GetBuildingFromPool, ReturnBuildingToPool));
        }
    }

    private PlacedBuildingBase CreatePoolBuildingType(BuildingType buildingType) //Method to create a Pool with the buildingType passed
    {
        BuildingData dataToUse = GetBuildingData(buildingType);
        PlacedBuildingBase newPooledBuilding;
        newPooledBuilding = Instantiate(dataToUse.BuildingPlacedPrefab, _buildingsGrp.transform);

        return newPooledBuilding; //Does this mean a single instance of object is created or a Pool was created? ASK THIS
    }

    private BuildingData GetBuildingData(BuildingType buildingType) //Method that returns data from buildingType passed
    {
        return _allBuildingData.Data.FirstOrDefault(b => b.KindOfStructure == buildingType);
    }

    private void GetBuildingFromPool(PlacedBuildingBase building) //Enable available building from placedBuildingBase passed
    {
        building.gameObject.SetActive(true);
    }
    private void ReturnBuildingToPool(PlacedBuildingBase building) //Disable available building from placedBuildingBase
    {
        building.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by the <see cref="BuildingPlacementUI"/>
    /// </summary>
    public void OnNewBuildingSelected(BuildingData building)
    {
        _buildingToPlace = building;
    }

    /// <summary>
    /// If we have a <see cref="_buildingToPlace"/> then show the ghost for it at the mouse position
    /// This will need to calculate where ground is.
    /// </summary>

    private void PlaceBuilding(Vector3 loc)
    {
        if (_buildingToPlace != null)
        {
            var building = _buildingsPools[_buildingToPlace.KindOfStructure].Get();
            
            building.transform.position = loc;
            _localPlayerBuildingManager.AddBuilding(building);
            var cellsCovered = _gameManager.GameGrid.GetCellsAroundPosition(loc, building._buildingData.BuildingSize);
            foreach (var cell in cellsCovered)
            {
                Debug.Log($"GridCell: {cell} covered by {building._buildingData.name}");
                //var cellsInGrid = _gameManager.GameGrid.GetGridCell(cell);
                //cellsInGrid.AddBuildingToCell(building);
                cell.AddBuildingToCell(building);
            }

            _placedParticles.transform.position = loc;
            _placedParticles.Play();
        }
    }

    public void BuildingOptions(PlacedBuildingBase buildingPlaced)
    {
        var dialog = _uiManager.ShowDialog(RTSMenus.BuildingOptions);
        if(dialog is BuildingOptions options)
        {
            options.Show(buildingPlaced._buildingData.name + " Options",
                "What do you want to do?",
                "Upgrade",
                "Dismantle",
                "Cancel",
                RemoveBuilding, buildingPlaced);
        }
    }

    public void RemoveBuilding(PlacedBuildingBase buildingToRemove)
    {
        buildingToRemove.gameObject.SetActive(false);
        _buildingsPools[buildingToRemove._buildingData.KindOfStructure].Release(buildingToRemove);
    }

    internal void SetLocalBuildingManager(PlayerBuildingsManager playerBuildingsManager)
    {
        _localPlayerBuildingManager = playerBuildingsManager;
    }


    internal void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void OnDrawGizmos()
    {
        if (_gameManager != null && _gameManager.GameGrid != null)
        {
            Gizmos.color = Color.yellow;

            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, 20000, _groundMask))
            {
                var pos = _gameManager.GameGrid.GetCellWorldCenter(hitInfo.point);

                Gizmos.DrawWireCube(pos, Vector3.one * _gameManager.GameGrid.CellSize);
            }
        }
    }
}

