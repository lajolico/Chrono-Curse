using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AttackChecker : MonoBehaviour
{
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        // Checks to see if player is within attack range
        switch (collision.gameObject.tag) 
        {
            case "Player":
                isInRange = true;
                UpdateAttackState();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Checks to see if player is outside attack range
        switch (collision.gameObject.tag)
        {
            case "Player":
                isInRange = false;
                UpdateAttackState();
                break;
        }
    }

    private void UpdateAttackState()
    {
        //Enemy_Logan enemyComponent = GetComponentInParent<Enemy_Logan>();
        // Puts enemy into appropriate attack state as needed
        Enemy_Logan enemyComponent = GetComponentInParent<Enemy_Logan>();
        if (enemyComponent != null)
        {
            enemyComponent.AttackingPlayer(isInRange);
        } 
    }

    public void EnemyAttacksPlayer() // Uses enemy animation controller to make attack results on player show at the appropriate time
    {
        Enemy enemyComponent = GetComponentInParent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.DoPlayerDamage();
        } 
    }
}
