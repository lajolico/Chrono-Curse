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

    private string enemySaveFile = "/enemies.json";

    private static string dungeonSavePath;

    private static string playerSavePath;

    private static string enemySavePath;

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

        enemySavePath = Application.persistentDataPath + enemySaveFile;
     }

    //Disallow external implementation of our class
    private SaveManager() { }

    public SaveDungeonData GetDungeonData()
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


    public SavePlayerData GetPlayerData()
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

    public SaveEnemyData GetEnemyData()
    {
        SaveEnemyData loadedGameData = null;
        if (File.Exists(Application.persistentDataPath + enemySaveFile))
        {
            try
            {
                string json = File.ReadAllText(Application.persistentDataPath + enemySaveFile);
                loadedGameData = JsonUtility.FromJson<SaveEnemyData>(json);
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

    public void DeleteEnemySave()
    {
        if(File.Exists(enemySavePath))
        {
            File.Delete(Application.persistentDataPath + enemySaveFile);
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
            string existingData = File.ReadAllText(playerSavePath);
            SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(existingData);
            if(!savePlayerData.playerData.isPlayerInDungeon)
            {
                return true;
            }
        }
        return false;
    }

    public bool PlayerDataExists()
    {
        if (File.Exists(playerSavePath))
        {
            return true;
        }
        return false;
    }

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

    public void SaveEnemyData(SaveEnemyData enemyData)
    {
        string json = JsonUtility.ToJson(enemyData);

        File.WriteAllText(enemySavePath, json);
    }

}

[System.Serializable]
public class SaveDungeonData { 

    public DungeonData dungeonData;
    public ExitPointData exitPointData;
    public PropData propData;


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

[System.Serializable]
public class SaveEnemyData
{
    public EnemyData enemyData;

    public SaveEnemyData(EnemyData enemyData)
    {
        this.enemyData = enemyData;
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
    public int kills;
    public int xp;
    public int damageGiven;
    public int damageTaken;
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


[System.Serializable]
public class EnemyData
{
    public List<EnemyState> enemies;
}

[System.Serializable]
public class EnemyState
{
    public Vector3 position;
    public int health;
    public string prefabName;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
    public int attackDamage;
    public float speed;
    public float attackRate;
    public float attackRange;
    public float nextAttackTime;
    public Enemy.EnemyType enemyType;
}
