using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Image _buildingSprite;
    [SerializeField] private TMP_Text _buildingName;

    private BuildingData _data;
    private BuildingPlacementManager _manager;

    public void Setup(BuildingData data, BuildingPlacementManager manager)
    {
        _data = data;
        _manager = manager;
        // setup ui of the button
        _buildingSprite.sprite = data.BuildingSprite;
        _buildingName.text = data.name;
    }

    public void OnButtonSelected()
    {
        _manager.OnNewBuildingSelected(_data);
    }
}

