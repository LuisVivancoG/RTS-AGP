using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _gameLvl;
    private bool _buttonPressed;

    private void Awake()
    {
        _buttonPressed = false;
    }

    public void LoadLevel()
    {
        if (!_buttonPressed)
        {
            StartCoroutine(NextLevel());
        }
    }

    public void QuitGame()
    {
        if (!_buttonPressed)
        {
            StartCoroutine(CloseGame());
        }
    }

    IEnumerator NextLevel()
    {
        _buttonPressed = true;
        AudioManager.Instance.UISound(AudioManager.UIType.ButtonClicked);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(_gameLvl);
    }

    IEnumerator CloseGame()
    {
        _buttonPressed = true;
        AudioManager.Instance.UISound(AudioManager.UIType.ButtonClicked);
        yield return new WaitForSeconds(1);
        Debug.Log("Closing game");
        Application.Quit();
    }
}
