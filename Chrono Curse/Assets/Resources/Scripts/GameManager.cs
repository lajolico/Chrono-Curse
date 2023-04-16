using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private DungeonGenerator dungeonGenerator;

    private PlayerData playerData;
    private DungeonData dungeonData;
    private PropData propData;

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
        SceneManager.LoadScene("Dungeon");
        if (startNewGame)
        {
            StartCoroutine(NewGameCoroutine());
          
        }
        else
        {
            // Load the saved game data
            StartCoroutine(LoadSavedGameCoroutine(SaveManager.Instance.LoadGame()));
        }
    }

    public void DungeonComplete()
    {
        LoadRestAreaScene();
    }

    private void LoadRestAreaScene()
    {
        SceneManager.LoadScene("MainMenu");
        playerData = PlayerManager.Instance.GetPlayerData();
        SaveGameData saveData = new SaveGameData(playerData);
        SaveManager.Instance.SaveGame(saveData);
    }

    public void PlayerDied()
    {
        SaveManager.Instance.DeleteSaveGame();
        PlayerManager.Instance.ResetPlayerAttributes();

        SceneManager.LoadScene("MainMenu"); //TODO you died scene
    }


    private IEnumerator NewGameCoroutine()
    {
        yield return new WaitUntil(() => DungeonGenerator.Instance != null || PlayerManager.Instance != null || PropManager.Instance != null);

        DungeonGenerator.Instance.GenerateDungeon();
    }

    private IEnumerator LoadSavedGameCoroutine(SaveGameData saveData)
    {
        yield return new WaitUntil(() => DungeonGenerator.Instance != null || PlayerManager.Instance != null);
        DungeonGenerator.Instance.SetDungeonData(saveData.dungeonData, saveData.propData);
        PlayerManager.Instance.LoadPlayerData(saveData.playerData);
        ExitPoint.Instance.LoadExitPoint(saveData.exitPointData);
    }

    public void SaveDungeon()
    {
        DungeonData dungeonData = DungeonGenerator.Instance.GetDungeonData();
        PropData propData = PropManager.Instance.GetPropData();
        PlayerData playerData = PlayerManager.Instance.GetPlayerData();
        ExitPointData exitPointData = ExitPoint.Instance.GetExitPointData();

        SaveGameData saveData = new SaveGameData(playerData, propData, dungeonData, exitPointData);
        SaveManager.Instance.SaveGame(saveData);
    }

}
