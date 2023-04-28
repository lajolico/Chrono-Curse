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
        if (SaveManager.Instance.DungeonSaveExists() || SaveManager.Instance.isPlayerInRestArea())
        {
            TextForPlayerButton(true);
        }
        else
        {
            TextForPlayerButton(false);             
        }
    }

    public void TextForPlayerButton(bool saveExists)
    {
        var playButtonText = myPlayButton.GetComponentInChildren<TextMeshProUGUI>();

        if (saveExists)
        {
            playButtonText.text = "CONTINUE";
        }
        else
        {
            playButtonText.text = "PLAY";            
        }
    }

    public void OnPlayButtonClicked()
    {
        // GameManager.Instance.LoadDemoDungeon();

        if (SaveManager.Instance.DungeonSaveExists())
        {
            GameManager.Instance.LoadDungeonScene(false);

        }else if(SaveManager.Instance.isPlayerInRestArea()){

            GameManager.Instance.LoadRestAreaScene();
        }
        else
        {
            GameManager.Instance.LoadDungeonScene(true);
        }
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    
    }
}
