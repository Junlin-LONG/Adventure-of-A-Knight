using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

    // Outlets
    public GameObject panalMainMenu;
    public GameObject mainMenu;
    public GameObject endMenu;
    public GameObject skillMenu;

    public TMP_Text textHighestScoreInStartMenu;

    private void Awake()
    {
        instance = this;
        Hide();
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        endMenu.SetActive(false);
        skillMenu.SetActive(false);

        gameObject.SetActive(true);
        panalMainMenu.SetActive(true);
        mainMenu.SetActive(true);

        if (PlayerPrefs.HasKey("BestScore")) { 
            textHighestScoreInStartMenu.text = "Best Score: "+PlayerPrefs.GetInt("BestScore");
        }

        Time.timeScale = 0;
        //GameController.instance.isPause = true;
    }

    public void ShowEndMenu()
    {
        mainMenu.SetActive(false);
        skillMenu.SetActive(false);

        gameObject.SetActive(true);
        panalMainMenu.SetActive(true);
        endMenu.SetActive(true);

        Time.timeScale = 0;
        GameController.instance.isPause = true;
    }

    public void ShowSkillMenu()
    {
        mainMenu.SetActive(false);
        endMenu.SetActive(false);
        panalMainMenu.SetActive(false);

        skillMenu.SetActive(true);

        Time.timeScale = 0;
        GameController.instance.isPause = true;
    }

    public void Hide()
    {
        mainMenu.SetActive(false);
        endMenu.SetActive(false);
        panalMainMenu.SetActive(false);
        skillMenu.SetActive(false);

        Time.timeScale = 1;
        if (GameController.instance != null)
            GameController.instance.isPause = false;
    }
}
