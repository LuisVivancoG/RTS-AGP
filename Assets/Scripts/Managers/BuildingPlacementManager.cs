using System.Collections.Generic;
using UnityEngine;
using System;
using Pool;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.Pool;

/// <summary>
/// This is the controller for placing buildings.
/// It receives input via the <see cref="BuildingPlacementUI"/>
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private AllBuildingsData _allBuildingData;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _buildingsMask;
    internal AllBuildingsData _runTimeBuildingsData => _allBuildingData;

    private BuildingData _buildingToPlace = null;
    private GameObject _placementGhost = null;

    private Dictionary<string, GameObject> _ghostsObjectsPool = new();

    [SerializeField] private ParticleSystem _placedParticles;

    [SerializeField] private GameObject _buildingsGrp;
    private Dictionary<BuildingType, BuildingPool> _buildingsPools;

    [SerializeField] private UIManager _uiManager;

    //[SerializeField] private GameManager _gameManager;
    //private GameGrid _gameGrid;

    private GameGrid _gameGrid = new GameGrid(30, 30, 30);


    private void Start()
    {
        //_gameGrid = _gameManager._gameGrid();
        //_placedBuildingsPool = new Dictionary<BuildingType, BuildingPool>();
        //foreach (BuildingType type in Enum.GetValues(typeof (BuildingType)))
        //{
        //    _placedBuildingsPool.Add(type, new BuildingPool(() => CreatePoolBuildingType(type), GetBuildingFromPool, ReturnBuildingToPool));
        //}

        InitializePool();
    }

    internal void InitializePool()
    {

        _buildingsPools = new Dictionary<BuildingType, BuildingPool>();
        foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
        {
            //if(!_BuildingsPools.ContainsKey(BuildingType))
            _buildingsPools.Add(type, new BuildingPool(() => CreatePoolBuildingType(type), GetBuildingFromPool, ReturnBuildingToPool));
        }
    }

    private PlacedBuildingBase CreatePoolBuildingType(BuildingType buildingType)
    {
        BuildingData dataToUse = GetBuildingData(buildingType);
        PlacedBuildingBase newPooledBuilding = Instantiate(dataToUse.BuildingPlacedPrefab, transform);
        newPooledBuilding.transform.parent = _buildingsGrp.transform;

        return newPooledBuilding;
    }

    private BuildingData GetBuildingData(BuildingType buildingType)
    {
        return _allBuildingData.Data.FirstOrDefault(b => b.KindOfStructure == buildingType);
    }

    private void GetBuildingFromPool(PlacedBuildingBase building)
    {
        building.gameObject.SetActive(true);
    }
    private void ReturnBuildingToPool(PlacedBuildingBase building)
    {
        building.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by the <see cref="BuildingPlacementUI"/>
    /// </summary>
    public void OnNewBuildingSelected(BuildingData building)
    {
        _buildingToPlace = building;
        //_buildingsGhostsPool.Add(_buildingToPlace.BuildingGhostPrefab);
    }

    /// <summary>
    /// If we have a <see cref="_buildingToPlace"/> then show the ghost for it at the mouse position
    /// This will need to calculate where ground is.
    /// </summary>
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_buildingToPlace == null)
        {
            if (Physics.Raycast(ray, out RaycastHit hitBuildingsInfo, 300, _buildingsMask) && Input.GetMouseButtonDown(0))
            {
                //var buildingClicked = hitBuildingsInfo.transform.GetComponent<PlacedBuildingsBase>();
                //_uiManager.OptionsEnable(buildingClicked);
                //BuildingOptions(buildingClicked);

                if (BuildingOptions(ray))
                {
                    return;
                }
            }

            return;
        }


        if (Physics.Raycast(ray, out RaycastHit hitInfo, 300, _groundMask))
        {
            if (_placementGhost != null)
            {
                _placementGhost.SetActive(false);
            }

            if (_ghostsObjectsPool.TryGetValue(_buildingToPlace.BuildingGhostPrefab.name, out var existingGhost))
            {
                _placementGhost = existingGhost;
                _placementGhost.SetActive(true);
            }

            else
            {
                _placementGhost = Instantiate(_buildingToPlace.BuildingGhostPrefab, transform);
                _ghostsObjectsPool.Add(_buildingToPlace.BuildingGhostPrefab.name, _placementGhost);
            }

            _placementGhost.transform.position = _gameGrid.GetCellWorldCenter(hitInfo.point);
            

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding(_gameGrid.GetCellWorldCenter(hitInfo.point));

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

    private void PlaceBuilding(Vector3 loc)
    {
        if (_buildingToPlace != null)
        {
            _placedParticles.transform.position = loc;
            _placedParticles.Play();
            PlacedBuildingBase go = Instantiate(_buildingToPlace.BuildingPlacedPrefab, loc, Quaternion.identity);
            go.transform.parent = _buildingsGrp.transform;
            //go.layer = _buildingsMask;
        }
    }

    public bool BuildingOptions(/*PlacedBuildingsBase buildingPlaced*/ Ray ray)
    {
        //buildingPlaced.gameObject.SetActive(false);

        //todo display a options popup with dismantling and upgrade buttons as well as other data info

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 300, _buildingsMask))
        {
            Debug.Log($"Dismantling building: {hitInfo.collider.gameObject.name}");
            hitInfo.collider.gameObject.SetActive(false);
            return true;
        }
        Debug.Log("No building hit");
        return false;
    }
}

