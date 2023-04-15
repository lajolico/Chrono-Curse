using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private bool isCrashSaveExists = false;
    private string crashSaveFileName = "CrashSave.json";

    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else if(_instance != this)
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
       
    }

    /// <summary>
    /// In charge of reseting our game.
    /// </summary>
    public void ResetGame()
    {

    }

    /// <summary>
    /// Start out game
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Dungeon");

    } 

    public void SaveGame()
    {
        //SaveManager.Instance.SaveGame();
    }

    public void RestAreaStart()
    {
        SceneManager.LoadScene("RestArea");
    }

    /// <summary>
    /// End our game
    /// </summary>
    public void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }



        
}
