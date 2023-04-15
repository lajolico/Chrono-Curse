using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public PlayerControls PlayerControl;
    public InputAction pause;

    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;

    private void Awake()
    {
        PlayerControl = new PlayerControls();
    }

    private void OnEnable()
    {
        pause = PlayerControl.UI.Pause;
        pause.Enable();
    }
    private void OnDisable()
    {
        pause.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        if (pause.triggered)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Debug.Log("Pausing!");
                Pause();
            }
        }
    }
    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
