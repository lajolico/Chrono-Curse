using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private PlayerData playerData;
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
     

    public void LoadDungeonScene(bool startNewGame)
    {
        SceneManager.LoadScene("LoadingScreen");

        StartCoroutine(WaitForLoadingScreen());

        if (startNewGame)
        {
            StartCoroutine(NewGame());
        }
        else
        {
            // Load the saved game data
            StartCoroutine(LoadDungeonGameCoroutine(SaveManager.Instance.LoadDungeon(), SaveManager.Instance.LoadPlayerData()));
        }
    }

    private IEnumerator WaitForLoadingScreen()
    {
        while(!SceneManager.GetSceneByName("LoadingScreen").isLoaded)
        {
            yield return null;
        }

        SceneLoader.Instance.LoadSceneAsync("Dungeon");
    }

    private IEnumerator WaitForRestLoadingScreen()
    {
        while (!SceneManager.GetSceneByName("LoadingScreen").isLoaded)
        {
            yield return null;
        }

        SceneLoader.Instance.LoadSceneAsync("RestArea");
    }

    public void LoadRestAreaScene()
    {

        if (!SceneManager.GetSceneByName("LoadingScreen").isLoaded)
        {
            SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
        }

        StartCoroutine(WaitForRestLoadingScreen());

        //SceneManager.LoadScene("RestArea");
        SaveManager.Instance.DeleteDungeonSave();
        if(SaveManager.Instance.isPlayerInRestArea())
        {
            PlayerManager.Instance.SpawnPlayer();
        }
        PlayerManager.Instance.SetPlayerInDungeon(false);
        PlayerManager.Instance.SetPlayerPosition(new Vector3(1, 1, 0));
        playerData = PlayerManager.Instance.GetPlayerData();
        SavePlayerData saveData = new SavePlayerData(playerData);

        SaveManager.Instance.SavePlayerData(saveData);
    }

    public void PlayerDied()
    {
        SaveManager.Instance.DeleteDungeonSave();
        SaveManager.Instance.DeletePlayerSave();
        PlayerManager.Instance.ResetPlayerAttributes();
        PlayerManager.Instance.SetPlayerInDungeon(false);

        SceneManager.LoadScene("YouDied"); //TODO you died scene
    }


    private IEnumerator NewGame()
    {
        yield return new WaitUntil(() => DungeonGenerator.Instance != null);

        StartCoroutine(DungeonGenerator.Instance.GenerateDungeon());
    }

    private IEnumerator LoadDungeonGameCoroutine(SaveDungeonData saveDungeonData, SavePlayerData savePlayerData)
    {

        yield return new WaitUntil(() => DungeonGenerator.Instance != null || PlayerManager.Instance != null || PropManager.Instance != null);

        DungeonGenerator.Instance.SetDungeonData(saveDungeonData.dungeonData, saveDungeonData.propData);
        ExitPoint.Instance.LoadExitPoint(saveDungeonData.exitPointData);

        PlayerManager.Instance.LoadPlayerData(savePlayerData.playerData);

    }

    public void SaveDungeon()
    {
        DungeonData dungeonData = DungeonGenerator.Instance.GetDungeonData();
        PropData propData = PropManager.Instance.GetPropData();
        ExitPointData exitPointData = ExitPoint.Instance.GetExitPointData();

        SaveDungeonData saveData = new SaveDungeonData(propData, dungeonData, exitPointData);
        SaveManager.Instance.SaveDungeonLevel(saveData);

        PlayerData playerData = PlayerManager.Instance.GetPlayerData();
        SavePlayerData playerSaveData = new SavePlayerData(playerData);
        SaveManager.Instance.SavePlayerData(playerSaveData);
    }

}
