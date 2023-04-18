using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PigSmall : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    public GameObject smallPig;
    public int attackDamage = 10;
    public float attackRange = 1.0f;
    public float attackDelay = 2.0f;
    private float lastAttackTime;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object by tag
    }

    void Update()
    {
        AttackPlayer(); // Call the AttackPlayer method in the Update method for continuous attacking
    }

    void Die()
    {
        Destroy(smallPig);
        smallPig.transform.parent.gameObject.SetActive(false);
    }

    void AttackPlayer()
    {
        // Check if enough time has passed since the last attack
        if (Time.time - lastAttackTime >= attackDelay)
        {
            // Check if player is within attack range
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                // Perform attack logic, such as play attack animation, deal damage to player, etc.
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time; // Update last attack time
            }
        }
    }
}
