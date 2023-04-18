using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackDelay = 1f;
    private float lastAttackTime;

    private void Update()
    {
        // Check if enough time has passed since last attack
        if (Time.time - lastAttackTime >= attackDelay)
        {
            // Perform attack logic, such as playing attack animation, dealing damage, etc.
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        // Deal damage to player or perform other attack actions
        // For example: PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        //              if (playerHealth != null) playerHealth.TakeDamage(damage);
    }
}

