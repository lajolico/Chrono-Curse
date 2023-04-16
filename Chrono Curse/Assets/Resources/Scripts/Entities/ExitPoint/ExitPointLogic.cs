using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPointLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Are you sure you want to leave?");
         
            GameManager.Instance.DungeonComplete();
        }
    }
}
