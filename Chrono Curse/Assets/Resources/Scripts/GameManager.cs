using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameObject loadingScreen;

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
            //  DontDestroyOnLoad(gameObject);
        }else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }
     

    public void LoadDungeonScene(bool startNewGame)
    {
        // SceneManager.LoadScene(1);
        if (startNewGame)
        {
            StartCoroutine(NewGame());
        }
        else
        {
            // Load the saved game data
            StartCoroutine(LoadDungeonGameCoroutine(SaveManager.Instance.GetDungeonData(),
                                                   SaveManager.Instance.GetPlayerData(),
                                                   SaveManager.Instance.GetEnemyData()));
        }
        // loadingScreen.SetActive(false);
    }

    // private IEnumerator WaitForLoadingScreen()
    // {
    //     while(!SceneManager.GetSceneByName("LoadingScreen").isLoaded)
    //     {
    //         yield return null;
    //     }

    //     
    // }

    // private IEnumerator WaitForRestLoadingScreen()
    // {
    //     while (!SceneManager.GetSceneByName("LoadingScreen").isLoaded)
    //     {
    //         yield return null;
    //     }

    //     SceneLoader.Instance.LoadSceneAsync("RestRoom");
    // }

    public void LoadRestAreaScene()
    {
        SceneManager.LoadScene("RestRoom");
        SaveManager.Instance.DeleteDungeonSave();
        SaveManager.Instance.DeleteEnemySave();
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
        SaveManager.Instance.DeleteEnemySave();
        PlayerManager.Instance.ResetPlayerAttributes();
        PlayerManager.Instance.SetPlayerInDungeon(false);
        SceneManager.LoadScene("YouDied");
        SaveManager.Instance.DeletePlayerSave();
    }

    private IEnumerator NewGame()
    {
        yield return new WaitUntil(() => DungeonGenerator.Instance != null);

        if(SaveManager.Instance.GetPlayerData() != null)
        {
            PlayerManager.Instance.LoadPlayerData(SaveManager.Instance.GetPlayerData().playerData);
        }

        DungeonGenerator.Instance.GenerateDungeon();
    }

    private IEnumerator LoadDungeonGameCoroutine(SaveDungeonData saveDungeonData, SavePlayerData savePlayerData, SaveEnemyData saveEnemyData)
    {
        yield return new WaitUntil(() => DungeonGenerator.Instance != null && PlayerManager.Instance != null 
        && PropManager.Instance != null && EnemyManager.Instance != null && AStarEditor.Instance != null);

        DungeonGenerator.Instance.SetDungeonData(saveDungeonData.dungeonData, saveDungeonData.propData);
        EnemyManager.Instance.LoadEnemies(saveEnemyData.enemyData);
        AStarEditor.Instance.LoadGraphData();
        ExitPoint.Instance.LoadExitPoint(saveDungeonData.exitPointData);
        PlayerManager.Instance.LoadPlayerData(savePlayerData.playerData);
    }

    public void SaveDungeon()
    {
        DungeonData dungeonData = DungeonGenerator.Instance.GetDungeonData();
        PropData propData = PropManager.Instance.GetPropData();
        ExitPointData exitPointData = ExitPoint.Instance.GetExitPointData();
        EnemyData enemyData = EnemyManager.Instance.GetEnemyData();

        SaveDungeonData saveData = new SaveDungeonData(propData, dungeonData, exitPointData);
        SaveManager.Instance.SaveDungeonLevel(saveData);

        SaveEnemyData saveEnemyData = new SaveEnemyData(enemyData);
        SaveManager.Instance.SaveEnemyData(saveEnemyData);

        PlayerData playerData = PlayerManager.Instance.GetPlayerData();
        SavePlayerData playerSaveData = new SavePlayerData(playerData);
        SaveManager.Instance.SavePlayerData(playerSaveData);
    }

}
