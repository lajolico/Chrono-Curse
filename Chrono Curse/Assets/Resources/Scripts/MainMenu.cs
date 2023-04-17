using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button myPlayButton;

    private void Awake()
    {
        var playButtonText = myPlayButton.GetComponentInChildren<TextMeshProUGUI>();

        if (SaveManager.Instance.DungeonSaveExists() || SaveManager.Instance.isPlayerInRestArea())
        {
            playButtonText.text = "CONTINUE";
        }
        else
        {
            playButtonText.text = "PLAY";
        }

    }

    /// <summary>
    /// Handle our Play Button Logic
    /// </summary>
    public void OnPlayButtonClicked()
    {
        if (SaveManager.Instance.DungeonSaveExists())
        {
            GameManager.Instance.LoadDungeonScene(false);

        }else if(SaveManager.Instance.isPlayerInRestArea())
        {
            GameManager.Instance.LoadRestAreaScene();
        }
        else
        {
            GameManager.Instance.LoadDungeonScene(true);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
