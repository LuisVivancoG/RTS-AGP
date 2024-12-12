using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUnitButton : MonoBehaviour
{
    [SerializeField] private Image _unitSprite;
    [SerializeField] private TMP_Text _unitName;

    private UnitsData _data;
    private ArmyCampOptions _armyCampOptions;

    public void Setup(UnitsData data, ArmyCampOptions ArmyOptionsUI)
    {
        _data = data;
        _unitSprite.sprite = data.UnitSprite;
        _unitName.text = data.name;
        _armyCampOptions = ArmyOptionsUI;
    }

    public void OnButtonSelected()
    {
        _armyCampOptions.UnitSelected(_data);
    }
}
