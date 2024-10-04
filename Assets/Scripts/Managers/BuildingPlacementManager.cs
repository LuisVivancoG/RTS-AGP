using System.Collections.Generic;
using UnityEngine;
using System;
using Pool;
using Unity.VisualScripting;

/// <summary>
/// This is the controller for placing buildings.
/// It receives input via the <see cref="BuildingPlacementUI"/>
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private AllBuildingsData _allBuildingData;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _buildingsMask;
    public AllBuildingsData AllBuildings => _allBuildingData;

    private BuildingData _buildingToPlace = null;
    private GameObject _placementGhost = null;

    private Dictionary<string, GameObject> _ghostsObjectsPool = new();

    [SerializeField] private ParticleSystem _placedParticles;

    [SerializeField] private GameObject _buildingsGrp;
    private BuildingPool _placedBuildingPool;

    [SerializeField] private UIManager _uiManager; 

    //private void Start()
    //{
    //    _placedBuildingPool = new BuildingPool();
    //}

    //public void GetBuildingFromPool(PlacedBuildingsBase)
    //{
    //    buildingToFetch.gameObject.SetActive(true);
    //}

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

            _placementGhost.transform.position = hitInfo.point;

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding(hitInfo.point);
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
            GameObject go = Instantiate(_buildingToPlace.BuildingPlacedPrefab, loc, _buildingToPlace.BuildingPlacedPrefab.transform.rotation);
            go.transform.parent = _buildingsGrp.transform;
            //go.layer = _buildingsMask;
        }
    }

    public bool BuildingOptions(/*PlacedBuildingsBase buildingPlaced*/ Ray ray)
    {
        //buildingPlaced.gameObject.SetActive(false);

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

