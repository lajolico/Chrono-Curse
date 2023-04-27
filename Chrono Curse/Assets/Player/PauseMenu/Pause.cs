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
    public bool freezeTime = false;

    public Animator myAnimator;
    public AppearingButtons appearButts;

    void Start()
    {

        pauseButton.GetComponent<Button>().onClick.AddListener(delegate { Pausing(); });
        pauseScreen.GetComponent<Button>().onClick.AddListener(delegate { closeBook(); });
    }

    public void Pausing()
    {
        pauseUI.SetActive(true);
        if (freezeTime)
        {
            Time.timeScale = 0f;
        }
    }

    // Allows book to close before game is resumed
    public void closeBook()
    {
        appearButts.disableButton();
        myAnimator.SetTrigger("Close");
    }

    // Is triggered from ShowHide so that the book's animation triggers the game resuming
    public void Resume()
    {
        pauseUI.SetActive(false);
        if (freezeTime)
        {
            Time.timeScale = 1f;
        } 
    }
}
