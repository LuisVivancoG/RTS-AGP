using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationDialog : DialogBase
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _acceptButtonText;
    [SerializeField] private TMP_Text _cancelButtonText;
    private Action<PlacedBuildingBase> _onDismantle;
    private PlacedBuildingBase _building;
    //[SerializeField] private Button _acceptButton;

    public override RTSMenus MenuType()
    {
        return RTSMenus.ConfirmationDialog;
    }

    //private Action _onConfirm;
    //private Action _onCancel;

    public void Show(string title, string description, string acceptButtonText, string cancelButtonText, Action<PlacedBuildingBase> dismantle, /*Action upgrade,*/ PlacedBuildingBase building)
    {
        _titleText.text = title;
        _descriptionText.text = description;
        _acceptButtonText.text = acceptButtonText;
        _cancelButtonText.text = cancelButtonText;
        _onDismantle = dismantle;
        _building = building;
        /*_onConfirm = onConfirm;
        _onCancel = onCancel;*/
    }

    public void ButtonAccept()
    {
        //_manager.PlayAudio(AudioIds.ConfirmButtonPressed);
        //_onConfirm?.Invoke();
        BuildingPlacementUI.Instance.gameObject.SetActive(true);
        _onDismantle?.Invoke(_building);
        _building.ToggleCanvas();
        _manager.HideDialog(MenuType());
        AudioManager.Instance.UISound(AudioManager.UIType.Accept);
    }
    public void ButtonCancel()
    {
        //_onCancel?.Invoke();
        BuildingPlacementUI.Instance.gameObject.SetActive(true);
        _building.ToggleCanvas();
        _manager.HideDialog(MenuType());
        AudioManager.Instance.UISound(AudioManager.UIType.Reject);
    }
}
