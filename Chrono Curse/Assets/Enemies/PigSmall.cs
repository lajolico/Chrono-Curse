// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Pathfinding;

// public class PigSmall : MonoBehaviour
// {
//     public int maxHealth = 100;
//     int currentHealth;

//     public GameObject smallPig;
//     public GameObject enemyAI;

//     private bool isInRange = false;

//     private void OnTriggerStay2D(Collider2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             isInRange = true;
//             // Debug.Log("Attack!!!");
//             enemyAI.GetComponent<EnemyAI>().AttackingPlayer(isInRange);
//         }
//     }
    
//     private void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             isInRange = false;
//             // Debug.Log("Not attacking...");
//             enemyAI.GetComponent<EnemyAI>().AttackingPlayer(isInRange);
//         }
//     }

//     void Die()
//     {
//         Destroy(smallPig);
//         smallPig.transform.parent.gameObject.SetActive(false);
//     }
// }
