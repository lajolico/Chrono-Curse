using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseUI;
    public GameObject pauseButton;
    public GameObject pauseScreen;

    void Start()
    {

        pauseButton.GetComponent<Button>().onClick.AddListener(delegate { Pausing(); });
        pauseScreen.GetComponent<Button>().onClick.AddListener(delegate { Resume(); });
    }

    public void Pausing()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
