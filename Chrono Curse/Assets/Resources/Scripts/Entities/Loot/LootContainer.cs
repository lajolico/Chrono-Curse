using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Purpose: Will be placed on our containers i.e. chests, boxes, etc.
public class LootContainer : MonoBehaviour
{
    //Our list of loot to spawn
    [SerializeField]
    private List<Loot> lootList;

    //Variables that will assist in the spawning and offset of our loot
    [SerializeField]
    private LayerMask wallLayer;

    [SerializeField] private float dropDistance = 0.5f;
    [SerializeField] private float spreadRadius = 0.5f;
    [SerializeField] private float dropOffset = 1f;


    /// <summary>
    /// Drops loot from the lootItems list at the given position, with the specified amount.
    /// </summary>
    public void DropLoot(Vector3 spawnPosition)
    {
        Vector3 playerPosition = PlayerManager.Instance.GetPlayerPosition();

        foreach (Loot loot in lootList)
        {
            if (loot.Drop())
            {
                for(int i = 0; i < loot.GetAmount(); i++)
                {
                    // Calculate the direction from the spawn position to the player position
                    Vector2 directionToPlayer = (playerPosition - spawnPosition).normalized;

                    RaycastHit2D hit = Physics2D.Raycast(spawnPosition, directionToPlayer, Mathf.Infinity, wallLayer);

                    Vector2 dropPosition;

                    if (hit.collider != null && hit.collider.gameObject.layer == wallLayer)
                    {
                        // If there is a wall, calculate a new drop position that is outside of the wall and towards the player
                        dropPosition = hit.point + hit.normal * 0.1f; // Offset the drop position slightly from the wall
                    }
                    else
                    {
                        dropPosition = spawnPosition;
                    }
          
                    // Calculate a random offset from the drop position
                    Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;

                    // Calculate the position in front of the player
                    Vector2 positionInFrontOfPlayer = (Vector2)playerPosition + directionToPlayer * dropOffset;

                    // Set the drop position towards the position in front of the player
                    dropPosition += (positionInFrontOfPlayer - dropPosition).normalized * dropDistance + randomOffset;
 
                    LootManager.Instance.SpawnLoot(loot, dropPosition);
                }
            }
        }
    }
     
}
