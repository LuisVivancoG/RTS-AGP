using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsHUD : MonoBehaviour
{
    [SerializeField] private BuildingPlacementManager _bpm;
    [SerializeField] private UIManager _uiManager;

    public void OnButtonDismantle(PlacedBuildingBase buildingPlaced)
    {
        //_bpm.BuildingOptions();
    }
}
