using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _safeZone;
    [SerializeField] private ConfirmationDialog _confirmationDialogPrefab;
    [SerializeField] private BuildingOptions _buildingOptionsPrefab;
    [SerializeField] private ArmyCampOptions _armyCampOptionsPrefab;
    [SerializeField] private PauseMenu _pauseOptionsPrefab;

    Dictionary<RTSMenus, DialogBase> _dialogInstances = new();

    Stack<DialogBase> _dialogStack = new Stack<DialogBase>();
    Dictionary<RTSMenus, DialogBase> _disabledDialogs = new ();

    private int _topSortingOrder = 0;
    private const int _sortOrderGap = 10;

    public DialogBase ShowDialog(RTSMenus dialogType)
    {
        return PushDialog(dialogType);
    }

    public DialogBase PushDialog(RTSMenus dialogType)
    {
        if (!_dialogInstances.ContainsKey(dialogType))
        {
            DialogBase created = null;
            switch (dialogType)
            {
                case RTSMenus.ConfirmationDialog:
                    created = CreateDialogFromPrefab(_confirmationDialogPrefab);
                    break;
                case RTSMenus.BuildingOptions:
                    created = CreateDialogFromPrefab(_buildingOptionsPrefab);
                    break;
                case RTSMenus.ArmyCampOptions:
                    created = CreateDialogFromPrefab(_armyCampOptionsPrefab);
                    break;
                case RTSMenus.PauseMenu:
                    created = CreateDialogFromPrefab(_pauseOptionsPrefab);
                    break;
            }
            if (created == null)
            {
                Debug.LogError($"Could not created dialog from prefab: {dialogType}");
            }
            else
            {
                _dialogInstances.Add(dialogType, created);
            }
        }
        DialogBase instance = _dialogInstances[dialogType];
        if (_dialogStack.Contains(instance))
        {
            Debug.LogError($"Dialog is already pushed: {dialogType}");
        }
        else
        {
            if (_disabledDialogs.ContainsKey(dialogType))
            {
                _disabledDialogs.Remove(dialogType);
            }
            _dialogStack.Push(instance);
            instance.gameObject.SetActive(true);
            instance.DialogCanvas.overrideSorting = true;
            _topSortingOrder += _sortOrderGap;
            instance.DialogCanvas.sortingOrder = _topSortingOrder;
        }
        return instance;
    }

    private DialogBase CreateDialogFromPrefab(DialogBase dialogPrefab)
    {
        DialogBase created = Instantiate(dialogPrefab , _safeZone.transform);
        created.OnCreation(this);
        return created;
    }

    public void HideDialog(RTSMenus dialogType)
    {
        PopDialog(dialogType);
    }

    private void PopDialog(RTSMenus dialogType)
    {
        if (!_dialogInstances.ContainsKey(dialogType))
        {
            Debug.LogError($"Tried to pop dialog, but dialog was never created {dialogType}");
            return;
        }
        DialogBase instance = _dialogInstances[dialogType];

        if(_dialogStack.TryPeek(out DialogBase topDialogPeek))
        {
            if(topDialogPeek == instance)
            {
                DialogBase topDialog = _dialogStack.Pop();
                topDialog.gameObject.SetActive(false);
                _disabledDialogs.Add(topDialog.MenuType(), topDialog);
                _topSortingOrder -= _sortOrderGap;  
            }
            else
            {
                Debug.LogError($"Tried to pop the dialog type {dialogType} but the top dialog was {topDialogPeek.MenuType()}");
            }
        }
        else
        {
            Debug.LogError($"Failed to peek the top dialog");
        }
    }
}
