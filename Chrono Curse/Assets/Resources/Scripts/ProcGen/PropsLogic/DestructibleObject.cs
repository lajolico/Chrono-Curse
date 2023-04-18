using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public bool isInteractProp = false;
    private int maxHealth;
    private int currentHealth;


    private void Start()
    {
        maxHealth = Random.Range(0, 4);
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(currentHealth <= 0 ) 
            {
                //This if statement ensures that we don't double drop loot, i.e. the chest, when it is destroyed.
                if(!isInteractProp)
                {
                    GetComponent<LootContainer>().DropLoot(transform.position);
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Attacked!");
                currentHealth -= PlayerManager.Instance.attackDamage;
            }
        }
    }
}
