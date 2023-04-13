using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite openSprite, closedSprite;

    private bool hasSpawnedLoot = false;

    private bool isOpen = false;


    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sprite = closedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        // Check if the player has entered the trigger collider
        if (other.CompareTag("Player"))
        {
            // If the chest is closed, open it and spawn loot if it hasn't already been spawned
            if (!isOpen)
            {
                OpenChest();
                if (!hasSpawnedLoot)
                {
                    hasSpawnedLoot = true;
                    GetComponent<LootContainer>().DropLoot(transform.position);
                }
            }
        }
    }
     

    private void OpenChest()
    {
        // Set the chest to open state
        isOpen = true;

        spriteRenderer.sprite = openSprite;
    }
 
}
