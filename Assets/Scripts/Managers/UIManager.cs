using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _buildingsMenu;
    [SerializeField] private GameObject _buildingOptions;

    public void OptionsEnable (PlacedBuildingBase building)
    {
        _buildingsMenu.SetActive(false);
        _buildingOptions.SetActive(true);
    }
    public void OptionsDisable()
    {
        _buildingsMenu.SetActive(true);
        _buildingOptions.SetActive(false);
    }
}
