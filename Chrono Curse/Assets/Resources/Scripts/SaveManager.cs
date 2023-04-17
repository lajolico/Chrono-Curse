using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    public static SaveManager Instance { get; private set; }

    private string dungeonSaveFile = "/dungeonsave.json";

    private string playerSaveFile = "/playerdata.json";

    private static string dungeonSavePath;

    private static string playerSavePath;

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

        dungeonSavePath = Application.persistentDataPath + dungeonSaveFile;

        playerSavePath = Application.persistentDataPath + playerSaveFile;
     }

    public SaveDungeonData LoadDungeon()
    {
        SaveDungeonData loadedGameData = null;
        if (File.Exists(Application.persistentDataPath + dungeonSaveFile))
        {
            try
            {
                string json = File.ReadAllText(Application.persistentDataPath + dungeonSaveFile);
                loadedGameData = JsonUtility.FromJson<SaveDungeonData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading game: " + e.Message);
            }
        }
        return loadedGameData;
    }

    public SavePlayerData LoadPlayerData()
    {
        SavePlayerData loadedGameData = null;
        if (File.Exists(Application.persistentDataPath + playerSaveFile))
        {
            try
            {
                string json = File.ReadAllText(Application.persistentDataPath + playerSaveFile);
                loadedGameData = JsonUtility.FromJson<SavePlayerData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading game: " + e.Message);
            }
        }
        return loadedGameData;
    }

    public void DeleteDungeonSave()
    {
        if (File.Exists(dungeonSavePath))
        {
            File.Delete(Application.persistentDataPath + dungeonSaveFile);
        }
    }

    public void DeletePlayerSave()
    {
        if (File.Exists(playerSavePath))
        {
            File.Delete(Application.persistentDataPath + playerSaveFile);
        }
    }

    public bool DungeonSaveExists()
    {
        if(File.Exists(dungeonSavePath))
        {
            return true;
        }

        return false;
    }

    public bool isPlayerInRestArea()
    {
        if(File.Exists(playerSavePath))
        {
            PlayerData playerData = new PlayerData();
            string existingData = File.ReadAllText(playerSavePath);
            SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(existingData);
            if(!savePlayerData.playerData.isPlayerInDungeon)
            {
                return true;
            }
        }
        return false;
    }

    //Disallow external implementation of our class
    private SaveManager() { }

    public void SavePlayerData(SavePlayerData newPlayerData)
    {
        if (File.Exists(playerSavePath))
        {
            // If the save file already exists, load the existing data and overwrite the playerData portion
            string existingData = File.ReadAllText(playerSavePath);
            SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(existingData);
            savePlayerData.playerData = newPlayerData.playerData;
        }

        string json = JsonUtility.ToJson(newPlayerData);

        File.WriteAllText(playerSavePath, json);
    }

    public void SaveDungeonLevel(SaveDungeonData saveData)
    {
        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(dungeonSavePath, json);
    }
}

[System.Serializable]
public class SaveDungeonData
{
    public ExitPointData exitPointData;
    public PropData propData;
    public DungeonData dungeonData;

    public SaveDungeonData (PropData propData, DungeonData dungeonData, ExitPointData exitPointData)
    {
        this.propData = propData;
        this.dungeonData = dungeonData;
        this.exitPointData = exitPointData;
    }
}

[System.Serializable]
public class SavePlayerData
{
    public PlayerData playerData;

    public SavePlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
    }
}

/// <summary>
/// Our player data
/// </summary>
[System.Serializable]
public class PlayerData
{
    public bool isPlayerInDungeon;
    public int health;
    public int gold;
    public float stamina;
    public int level;
    public Vector3 position;
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

[System.Serializable]
public class PropData
{
    public List<PropInfo> propSaveData = new List<PropInfo>();
}

[System.Serializable]
public class PropInfo
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public Sprite sprite;
    public bool hasCollider;
    public bool hasTrigger;
    public List<string> scriptNames = new List<string>();
    public Vector3 spritePosition;
    public Vector2 ColliderSize;

    public float triggerRadius;
    public Vector2 triggerOffset;
}


[System.Serializable]
public class ExitPointData
{
    public Vector3 position;  
}


public class EnemyData
{ 
    
}


