using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button myPlayButton;

    private void Awake()
    {
        Text playButtonText = myPlayButton.GetComponentInChildren<Text>();


        if (SaveManager.Instance.SaveFileExists())
        {
            playButtonText.text = "Continue";
        }
        else
        {
            playButtonText.text = "Play";
        }

    }

    public void OnPlayButtonClicked()
    {
        if(SaveManager.Instance.SaveFileExists())
        {
            GameManager.Instance.LoadDungeonScene(false);
        }else
        {
            GameManager.Instance.LoadDungeonScene(true);
        }
    }
}
