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

    public SaveGameData LoadGame()
    {
        SaveGameData loadedGameData = null;
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                loadedGameData = JsonUtility.FromJson<SaveGameData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading game: " + e.Message);
            }
        }
        return loadedGameData;
    }

    public void DeleteSaveGame()
    {
        if (File.Exists(savePath))
        {
            File.Delete(Application.persistentDataPath + saveFile);
        }
    }

    public bool SaveFileExists()
    {
        if(File.Exists(savePath))
        {
            return true;
        }

        return false;
    }

    //Disallow external implementation of our class
    private SaveManager() { }

    public void SaveGame(SaveGameData saveData)
    {

        if (File.Exists(savePath))
        {
            // If the save file already exists, load the existing data and overwrite the playerData portion
            string existingData = File.ReadAllText(savePath);
            SaveGameData saveGameData = JsonUtility.FromJson<SaveGameData>(existingData);
            saveGameData.playerData = saveData.playerData;
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(savePath, json);
    }
}

[System.Serializable]
public class SaveGameData
{
    public PlayerData playerData;
    public ExitPointData exitPointData;
    public PropData propData;
    public DungeonData dungeonData;

    public SaveGameData (PlayerData playerData, PropData propData = null, DungeonData dungeonData = null, ExitPointData exitPointData = null)
    {
        this.playerData = playerData;
        this.propData = propData;
        this.dungeonData = dungeonData;
        this.exitPointData = exitPointData;
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


