using System.Collections.Generic;
using UnityEngine;
using Pool;
using Unity.VisualScripting;

/// <summary>
/// This is the controller for placing buildings.
/// It receives input via the <see cref="BuildingPlacementUI"/>
/// </summary>
public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private AllBuildingsData _allBuildingData;
    [SerializeField] private LayerMask GroundMask;
    public AllBuildingsData AllBuildings => _allBuildingData;

    private BuildingData _buildingToPlace = null;
    private GameObject _placementGhost = null;

    private Dictionary<string, GameObject> _ghostsObjectsPool = new();

    [SerializeField] private ParticleSystem PlacedParticles;

    private BuildingPool _placedBuildingPool;

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
        if (_buildingToPlace == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 300, GroundMask))
        {
            PlacedParticles.transform.position = hitInfo.point;
            //Debug.Log(hitInfo.collider.name);

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
                PlaceBuilding();
                _buildingToPlace = null;
                _placementGhost.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1))
            {
                _buildingToPlace = null;
                _placementGhost.SetActive(false);
            }

            /*if (hitInfo.Equals(_buildingToPlace) && Input.GetKeyDown(KeyCode.Delete))
            {
                _buildingToPlace.GameObject.SetActive(false);
            }*/
        }
    }

    private void PlaceBuilding()
    {
        if (_buildingToPlace != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitInfo, 300, GroundMask);

            PlacedParticles.Play();
            GameObject go = Instantiate(_buildingToPlace.BuildingPlacedPrefab, hitInfo.point, _buildingToPlace.BuildingPlacedPrefab.transform.rotation);
        }
    }

    /*private void DismantleBuilding(BuildingData building)
    {
        building.GameObject.SetActive(false);
    }*/
}

