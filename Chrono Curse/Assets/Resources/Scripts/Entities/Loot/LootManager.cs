using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Logan Jolicoeur
public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    [SerializeField] private ObjectPooling lootPool;
    [SerializeField] private List<Loot> allLoot;

    private LootManager() { }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Spawns a new loot object at the given position and adds it to the list of all loot objects.
    /// </summary>
    /// <param name="loot">The loot object to spawn.</param>
    /// <param name="position">The position to spawn the loot object at.</param>
    public void SpawnLoot(Loot loot, Vector2 position)
    {
        GameObject newLoot = lootPool.GetObject();
        if(newLoot != null) 
        {
            newLoot.transform.position = position;
            newLoot.GetComponent<LootPickup>().SetLoot(loot);
            newLoot.SetActive(true);
            newLoot.GetComponent<LootPickup>().CollectAfterDelay(0.1f);
            allLoot.Add(loot);
        }
    }

    /// <summary>
    /// Collects the given loot object and removes it from the game world.
    /// </summary>
    /// <param name="lootPickup">The loot object to collect.</param>
    public void CollectLoot(LootPickup lootPickup)
    {
        if (allLoot.Contains(lootPickup.Loot))
        {
            if(lootPickup.Loot.minGoldAmount> 0) 
            {
                PlayerManager.Instance.SetGold(lootPickup.Loot.GenerateGoldAmount());
                Debug.Log("Player Gold :" + PlayerManager.Instance.Gold);
            }

            allLoot.Remove(lootPickup.Loot);
            lootPool.ReturnObject(lootPickup.gameObject);
            lootPickup.ResetState();
        }
    }
}
