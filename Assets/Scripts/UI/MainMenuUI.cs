using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _themeMenu;

    private void Start()
    {
        DataManager.Instance.RankIndexAchieved = -1;
        BackToMainMenu();
    }

    public void NewGame(int themeIndex)
    {
        DataManager.Instance.ActiveTheme = themeIndex;
        SceneManager.LoadScene(2);
    }

    public void SelectTheme()
    {
        _mainMenu.gameObject.SetActive(false);
        _themeMenu.gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        _mainMenu.gameObject.SetActive(true);
        _themeMenu.gameObject.SetActive(false);
    }

    public void HighScores()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
