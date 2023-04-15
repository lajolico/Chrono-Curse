using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    public static SaveManager Instance { get; private set; }

    private string savePath;

    private string saveFile = "savegame.json";

    private void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Path.Combine(Application.persistentDataPath, saveFile);
    }

    //Disallow external implementation of our class
    private SaveManager() { }

    public void SaveGameOnExit()
    {
        PlayerData playerData = PlayerManager.Instance.GetPlayerData();
        PropData propData = PropManager.Instance.GetPropData();
        DungeonData dungeonData = DungeonGenerator.Instance.GetDungeonData();

        SaveGameData saveData = new SaveGameData(playerData, propData, dungeonData);

        if (propData != null)
        {
            saveData.propData = propData;
        }

        if (dungeonData != null)
        {
            saveData.dungeonData = dungeonData;
        }

        // Convert the SaveGameData to a JSON string
        string json = JsonUtility.ToJson(saveData);

        // Save the JSON string to a file
        File.WriteAllText(savePath, json);
    }

    public void SaveGame(string fileName, SaveGameData saveData)
    {
        PlayerData playerData= PlayerManager.Instance.GetPlayerData();

        SaveGameData saveData = new SaveGameData(playerData);

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(savePath, json);
    }

    public void LoadGame(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveGameData saveData = JsonUtility.FromJson<SaveGameData>(json);

            // Load player data
            if (saveData.playerData != null)
            {
                PlayerManager.Instance.LoadPlayerData(saveData.playerData);
            }
            else
            {
                Debug.Log("No player data found in save file.");
            }

            // Load dungeon data
            if (saveData.dungeonData != null)
            {
                //DungeonGenerator.Instance.LoadRooms(saveData.dungeonData.rooms);
            }
            else
            {
                Debug.Log("No dungeon data found in save file.");
            }

            // Load prop data
            if (saveData.propData != null)
            {
                PropManager.Instance.LoadPropData(saveData.propData);
            }
            else
            {
                Debug.Log("No prop data found in save file.");
            }
        }
        else
        {
            Debug.Log("Save file does not exist.");
        }
    }
}

[System.Serializable]
public class SaveGameData
{
    public PlayerData playerData;
    public PropData propData;
    public DungeonData dungeonData;

    public SaveGameData (PlayerData playerData, PropData propData = null, DungeonData dungeonData = null)
    {
        this.playerData = playerData;
        this.propData = propData;
        this.dungeonData = dungeonData;
    }
}

/// <summary>
/// Our player data
/// </summary>
[System.Serializable]
public class PlayerData
{
    public int health;
    public int gold;
    public float stamina;
    public int level;
    public Vector3 position;
}

[System.Serializable]
public class PropData
{
    public List<GameObject> props = new List<GameObject>();
}

[System.Serializable]
public class DungeonData
{
    public List<TileSaveData> floorTiles;
    public List<TileSaveData> wallTiles;
}
 
[System.Serializable]
public class TileSaveData
{
    public Vector3Int position;
    public TileBase tileBase;

    public TileSaveData(Vector3Int position, TileBase tileBase)
    {
        this.position = position;
        this.tileBase = tileBase;
    }
}

