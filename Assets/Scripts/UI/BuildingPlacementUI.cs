using UnityEngine;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private BuildingPlacementManager _buildingPlacementManager;

    [SerializeField] private SelectBuildingButton _selectBuildingButton;
    [SerializeField] private Transform _scrollRectContent;

    private void Start()
    {
        foreach (var building in _buildingPlacementManager._runTimeBuildingsData.Data)
        {
            SelectBuildingButton button = Instantiate(_selectBuildingButton, _scrollRectContent);
            button.Setup(building, _buildingPlacementManager);
        }
    }
}
