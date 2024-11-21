using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : DialogBase
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _acceptButtonText;
    [SerializeField] private TMP_Text _cancelButtonText;
    //[SerializeField] private Button _acceptButton;

    public override RTSMenus MenuType()
    {
        return RTSMenus.ConfirmationDialog;
    }

    private UIManager _manager;

    private Action _onConfirm;
    private Action _onCancel;

    public void OnCreation(UIManager manager)
    {
        _manager = manager;
    }

    public void Show(string title, string description, string acceptButtonText, string cancelButtonText)
    {
        _titleText.text = title;
        _descriptionText.text = description;
        _acceptButtonText.text = acceptButtonText;
        _cancelButtonText.text = cancelButtonText;
    }

    public void ButtonAccept()
    {
        //_manager.PlayAudio(AudioIds.ConfirmButtonPressed);
        _onConfirm?.Invoke();
        _manager.HideDialog(MenuType());
    }
    public void ButtonCancel()
    {
        _onCancel?.Invoke();
        _manager.HideDialog(MenuType());
    }
}
