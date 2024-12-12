using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private RectTransform _placementManager;
    [SerializeField] private RectTransform _toggleButton;
    [SerializeField] private Image _pauseButtonImg;
    [SerializeField] private Image _resumeButtonImg;
    [SerializeField] private UIManager _uiManager;

    public void TogglePause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            _pauseButtonImg.enabled = true;
            _resumeButtonImg.enabled = false;
        }    

        else
        {
            Time.timeScale = 0;
            _pauseButtonImg.enabled = false;
            _resumeButtonImg.enabled = true;
            var dialog = _uiManager.ShowDialog(RTSMenus.PauseMenu);
            if (dialog is PauseMenu pauseMenu)
            {
                pauseMenu.Show(TogglePause);
            }
        }
    }

    public void ToggleBuildingsMenu()
    {
        Vector2 zero = new Vector2(0f, 0f);

        if(_placementManager.sizeDelta == zero)
        {
            _placementManager.sizeDelta = new Vector2(1200, 0);
            _toggleButton.anchoredPosition = new Vector3(0, 200, 0);
        }
        else
        {
            _placementManager.sizeDelta = zero;
            _toggleButton.anchoredPosition = new Vector3(275, 200, 0);
        }
    }
}
