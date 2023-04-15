using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public int Health { get; private set; } = 100;
    public int Gold { get; private set; } = 0;

    public int Level { get; private set; } = 1;

    public float Stamina { get; private set; } = 120.0f;

    [SerializeField]
    public GameObject playerPrefab;

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
        playerData.health =  Health;
        playerData.gold =  Gold;
        playerData.stamina =  Stamina;
        playerData.level =  Level;

        return playerData;
    }

    public void LoadPlayerData (PlayerData playerData)
    {
        playerInstance.transform.position = playerData.position;
        Health = playerData.health;
        Gold = playerData.gold;
        Stamina = playerData.stamina;
        Level = playerData.level;  
    }

}
