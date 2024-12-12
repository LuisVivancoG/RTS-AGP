using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyCampOptions : DialogBase
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _upgradeButtonText;
    [SerializeField] private TMP_Text _dismantleButtonText;
    [SerializeField] private TMP_Text _cancelButtonText;
    [SerializeField] private TMP_Text _unitSelectedText;
    [SerializeField] private SelectUnitButton _selectUnitButton;
    [SerializeField] private Transform _scrollRectContent;
    [SerializeField] private AllUnitsData _allUnitsData;
    public AllUnitsData AllUnitsData => _allUnitsData;

    public override RTSMenus MenuType()
    {
        return RTSMenus.ArmyCampOptions;
    }

    private Action _onUpgrade;
    private Action<PlacedBuildingBase> _onDismantle;
    private Action _onCancel;
    private PlacedBuildingBase _buildingSelected;

    private int _faction;
    private ArmyCamp _currentCamp;
    private UnitsBase _unitInQueue;

    private void Start()
    {
        foreach (var unit in AllUnitsData.Data)
        {
            SelectUnitButton button = Instantiate(_selectUnitButton, _scrollRectContent);
            button.Setup(unit, this);
        }
    }

    public void UnitSelected(UnitsData requestedData)
    {
        _unitInQueue = requestedData.UnitPrefab;
        _unitSelectedText.text = ($"{_unitInQueue.name} picked");
    }

    public void SetFaction(int currentFaction)
    {
        _faction = currentFaction;
    }

    public void Show(string title,
        string description,
        string unitSelected,
        string upgradeButtonText,
        string dismantleButtonText,
        string cancelButtonText,
        Action<PlacedBuildingBase> dismantle,
        /*Action upgrade,*/
        PlacedBuildingBase building)
    {
        _titleText.text = title;
        _descriptionText.text = description;
        _unitSelectedText.text = unitSelected;
        _dismantleButtonText.text = dismantleButtonText;
        _upgradeButtonText.text = upgradeButtonText;
        _cancelButtonText.text = cancelButtonText;
        //_onUpgrade = upgrade;
        _onDismantle = dismantle;
        _buildingSelected = building;
        _currentCamp = building.GetComponent<ArmyCamp>();
    }
    public void ButtonDismantle()
    {
        //_manager.PlayAudio(AudioIds.DismantleButton);
        //_onDismantle?.Invoke(_buildingSelected);
        _unitInQueue = null;
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

    public void ButtonSpawn()
    {
        if(_unitInQueue != null)
        {
            _currentCamp.RequestUnit(_unitInQueue);
            _unitInQueue = null;
        }
        else
        {
            _unitSelectedText.text = "No unit was selected. \n" +
                "First pick a troop to spawn, then click Generate";
        }
    }

    public void ButtonUpgrade()
    {
        //_manager.PlayAudio(AudioIds.UpgradeButton);
        //_onUpgrade?.Invoke();
        _unitInQueue = null;
        _manager.HideDialog(MenuType());
    }
    public void ButtonCancel()
    {
        //_onCancel?.Invoke();
        BuildingPlacementUI.Instance.gameObject.SetActive(true);
        _buildingSelected.ToggleCanvas();
        _unitInQueue = null;
        _manager.HideDialog(MenuType());
    }
}
