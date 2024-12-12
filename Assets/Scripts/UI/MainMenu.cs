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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void QuitGame()
    {
        if (!_buttonPressed)
        {
            StartCoroutine(CloseGame());
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
    }

    IEnumerator NextLevel()
    {
        _buttonPressed = true;
        AudioManager.Instance.UISound(AudioManager.UIType.ButtonClicked);
        yield return new WaitForSeconds(1);
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        SceneManager.LoadScene(_gameLvl);
    }

    IEnumerator CloseGame()
    {
        _buttonPressed = true;
        AudioManager.Instance.UISound(AudioManager.UIType.ButtonClicked);
        yield return new WaitForSeconds(1);
        Debug.Log("Closing game");
        //_buttonPressed = false;
        Application.Quit();
    }
}
