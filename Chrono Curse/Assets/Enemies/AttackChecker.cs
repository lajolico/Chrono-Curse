using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AttackChecker : MonoBehaviour
{
    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        Enemy enemyComponent = GetComponentInParent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.AttackingPlayer(isInRange);
        } 
    }

}
