using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PigSmall : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    public GameObject smallPig;

    void Die()
    {
        Destroy(smallPig);
        smallPig.transform.parent.gameObject.SetActive(false);
    }
}
