using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class DeathScreenManager : MonoBehaviour
{

    public TextMeshProUGUI Level, Kills, Gold;

    private void Start()
    {
        if(PlayerManager.Instance != null)
        {
            Level.text = "Level: " + PlayerManager.Instance.currentLevel.ToString();
            Kills.text = "Kills: " + PlayerManager.Instance.Kills.ToString();
            Gold.text = "Gold: " + PlayerManager.Instance.Gold.ToString();
        }else
        {
            Level.text = "Level: 0";
            Kills.text = "Kills: 0";
            Gold.text = "Gold: 0";
        }

    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        GameManager.Instance.QuitGame();
    }

}
