using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPointLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //GameManager.Instance.LoadRestAreaScene();
            GameManager.Instance.LoadDemoRestScene();
        }
    }
}
