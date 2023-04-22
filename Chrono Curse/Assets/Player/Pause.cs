using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{

    public static bool GameIsPaused = false;
    // public Button pauseButton;
    public GameObject pauseUI;
    public GameObject pauseButton;
    public GameObject pauseScreen;
    // public button toPauseMenu;
    // public button returnToGame;
    // // Start is called before the first frame update
    void Start()
    {
        // toPauseMenu.onClick.AddListener(delegate { Pausing(); });
        // returnToGame.onClick.AddListener(delegate { Resume(); });

        pauseButton.GetComponent<Button>().onClick.AddListener(delegate { Pausing(); });
        pauseScreen.GetComponent<Button>().onClick.AddListener(delegate { Resume(); });
    }
    // public void OnPlayButtonClicked()
    // {
    //     if (!GameIsPaused)
    //     {
    //         GameIsPaused = true;
    //         Pausing();
    //         // pauseScreen.SetActive(true);
    //     }
    //     else
    //     {
    //         GameIsPaused = false;
    //         Resume();
    //     }
        
    // }

    public void Pausing()
    {
        Debug.Log("burh");
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }


    // Update is called once per frame
//     void Update()
//     {
//         if
//     }
}
