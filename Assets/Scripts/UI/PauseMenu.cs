using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : DialogBase
{
    [SerializeField] private string _menuLvl;
    private Action _resume;
    public override RTSMenus MenuType()
    {
        return RTSMenus.PauseMenu;
    }

    public void Show(Action resumeAction)
    {
        _resume = resumeAction;
    }

    public void Resume()
    {
        _resume?.Invoke();
        _manager.HideDialog(MenuType());
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene(_menuLvl);
    }

    public void QuitGame()
    {
        Debug.Log("Closing game");
        Application.Quit();
        /*var dialog = _manager.ShowDialog(RTSMenus.ConfirmationDialog);
        if (dialog is ConfirmationDialog confirmation)
        {
            confirmation.Show("Are you sure you want to dismantle\n" + _buildingSelected._buildingData.name + "?",
                "You cannot undo this action.",
                $"Dismantle " + _buildingSelected._buildingData.name,
                "Cancel", _onDismantle, _buildingSelected);
            AudioManager.Instance.UISound(AudioManager.UIType.Confirmation);
        }*/
    }
}
