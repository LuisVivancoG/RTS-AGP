using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class BuildingOptions : DialogBase
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _upgradeButtonText;
    [SerializeField] private TMP_Text _dismantleButtonText;
    [SerializeField] private TMP_Text _cancelButtonText;
    //[SerializeField] private Button _upgradeButton;
    //[SerializeField] private Button _dismantleButton;
    //[SerializeField] private Button _cancelButton;

    public override RTSMenus MenuType()
    {
        return RTSMenus.BuildingOptions;
    }

    //private Action _onUpgrade;
    private Action<PlacedBuildingBase> _onDismantle;
    //private Action _onCancel;
    private PlacedBuildingBase _buildingSelected;

    public void Show(string title,
        string description,
        string upgradeButtonText,
        string dismantleButtonText,
        string cancelButtonText,
        Action<PlacedBuildingBase> dismantle,
        /*Action upgrade,*/
        PlacedBuildingBase building)
    {
        _titleText.text = title;
        _descriptionText.text = description;
        _dismantleButtonText.text = dismantleButtonText;
        _upgradeButtonText.text = upgradeButtonText;
        _cancelButtonText.text = cancelButtonText;
        //_onUpgrade = upgrade;
        _onDismantle = dismantle;
        _buildingSelected = building;
    }
    public void ButtonDismantle()
    {
        //_manager.PlayAudio(AudioIds.DismantleButton);
        //_onDismantle?.Invoke(_buildingSelected);
        _manager.HideDialog(MenuType());
        var dialog = _manager.ShowDialog(RTSMenus.ConfirmationDialog);
        if (dialog is ConfirmationDialog confirmation)
        {
            confirmation.Show("Are you sure you want to dismantle\n" + _buildingSelected._buildingData.name + "?",
                "You cannot undo this action.",
                $"Dismantle " + _buildingSelected._buildingData.name,
                "Cancel", _onDismantle, _buildingSelected);
            AudioManager.Instance.UISound(AudioManager.UIType.Confirmation);
        }
    }

    public void ButtonUpgrade()
    {
        //_manager.PlayAudio(AudioIds.UpgradeButton);
        //_onUpgrade?.Invoke();
        _manager.HideDialog(MenuType());
    }
    public void ButtonCancel()
    {
        //_onCancel?.Invoke();
        BuildingPlacementUI.Instance.gameObject.SetActive(true);
        _buildingSelected.ToggleCanvas();
        _manager.HideDialog(MenuType());
    }
}
