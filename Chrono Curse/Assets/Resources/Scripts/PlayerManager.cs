using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int Gold { get; private set; } = 0;
    public float Stamina { get; private set; } = 120.0f;
    public int Kills { get; private set; } = 0;
    public int currentXP { get; private set; } = 0;
    public int currentLevel { get; private set; } = 1;
    private int xpToLevelUp = 100;
    public int baseXPToLevelUp = 100;
    public float xpToLevelUpMultiplier = 1.5f;
    public int baseDamage = 20;

    public int attackDamage { get; private set; }

    public bool isPlayerInDungeon { get; private set; } = false;

    [SerializeField]
    public GameObject playerPrefab;
     
    private CinemachineVirtualCamera vCamera;

    //Ensure that our player persists between scene changes
    private GameObject playerInstance;

    private PlayerManager() { }

    // Awake method to initialize the singleton instance of the class
    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance of the class already exists, destroy this one
            Destroy(gameObject);
        }

       
    }

    // Public method to get the player's position
    public Vector3 GetPlayerPosition()
    {
        if (playerInstance == null)
        {
            // If the player's GameObject hasn't been set yet, return Vector2.zero
            Debug.LogWarning("Player instance does not exist, in the GetPlayerPosition()");
            return Vector3.zero;
        }
        else
        {
            // Otherwise, return the player's position
            return playerInstance.transform.position;
        }
    }

    // Public method to get the player's position
    public void SetPlayerPosition(Vector3 newPosition)
    {
        if (playerInstance != null)
        {
            playerInstance.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Player instance does not exist in SetPlayerPosition()");
        }
    }

    public void SpawnPlayer()
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(playerInstance);
        }
        else
        {
            Debug.LogWarning("Player instance already exists.");
        }
    }

    public void DestroyPlayer()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerInstance = null;
        }
    }

    public Transform GetPlayerTransform()
    {
        if (playerInstance != null)
        {
            return playerInstance.transform;
        }
        else
        {
            Debug.LogWarning("Player instance does not exist. GetTransform()");
            return null;
        }
    }

    // Public method to update the player's health
    public void SetHealth(int amount)
    {
        Health += amount;
    }

    // Public method to update the player's gold
    public void SetGold(int amount)
    {
        Gold += amount;
    }

    /// <summary>
    /// Used in our SaveManager, will save the current stats of our player
    /// </summary>
    /// <returns>Returns PlayerData</returns>
    public PlayerData GetPlayerData()
    {
        PlayerData playerData = new PlayerData();
        playerData.position = GetPlayerPosition();
        playerData.health =  MaxHealth;
        playerData.gold =  Gold;
        playerData.stamina =  Stamina;
        playerData.level =  currentLevel;
        playerData.isPlayerInDungeon = isPlayerInDungeon;
        playerData.kills = Kills;
        playerData.xp = currentXP;

        return playerData;
    }

    /// <summary>
    /// This is utilized when we want to reload our game from a SaveFile
    /// </summary>
    /// <param name="playerData">Our players data</param>
    public void LoadPlayerData (PlayerData playerData)
    {
        if(playerData.isPlayerInDungeon)
        {
            SpawnPlayer();
            SetPlayerPosition(playerData.position);
            SetPlayerCamera();
        }

        Health = playerData.health;
        Gold = playerData.gold;
        Stamina = playerData.stamina;
        currentLevel = playerData.level;
        currentXP = playerData.xp;
        Kills = playerData.kills;
        xpToLevelUp = Mathf.RoundToInt(baseXPToLevelUp * Mathf.Pow(xpToLevelUpMultiplier, currentLevel - 1));
        SetAttackDamage(currentLevel);
    }

    public void ResetPlayerAttributes()
    {
        Health = 100;
        Gold = 0;
        Stamina = 100.0f;
        currentLevel = 1;
    }

    public void SetPlayerCamera()
    {
        vCamera = playerInstance.GetComponentInChildren<CinemachineVirtualCamera>();
        vCamera.Follow = GetPlayerTransform();
        vCamera.LookAt = GetPlayerTransform();
    }

    public void SetPlayerInDungeon(bool isInDungeon)
    {
        isPlayerInDungeon = isInDungeon;
    }

    public void AddKill(int amount)
    {
        this.Kills++;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log("CurrentXP: " + currentXP);
        if (currentXP >= xpToLevelUp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXP = 0;
        xpToLevelUp = Mathf.RoundToInt(baseXPToLevelUp * Mathf.Pow(xpToLevelUpMultiplier, currentLevel - 1));
        SetAttackDamage(currentLevel);
    }

    public void SetAttackDamage(int playerLevel)
    {
        // Calculate the player's damage output based on their level
        float damageMultiplier = 1.0f + ((float)playerLevel / 20.0f); // Increase damage by 5% for every 2 player levels
        attackDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    public int GetLevel()
    {
        return currentLevel;
    }

    public void DamagePlayer(int damage)
    {
        this.Health -= damage;
    }

    public void SetPlayerHealthPerLevel(int level)
    {
        // Set the player's maximum health based on their level
        MaxHealth = 100 + (level * 10);
        // Set the player's current health to the new maximum health
        Health = MaxHealth;
    }

    public void HealPlayer(int amount)
    {
        StartCoroutine(HealOverTime(amount));
    }

    IEnumerator HealOverTime(int amount)
    {
        float healPerSecond = amount / 10f; // adjust the divisor to control the time it takes to heal
        while (Health < MaxHealth)
        {
            Health += (int) (healPerSecond * Time.deltaTime);
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
            yield return null;
        }
    }

}
