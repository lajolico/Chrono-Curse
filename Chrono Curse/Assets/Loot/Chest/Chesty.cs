using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chesty : MonoBehaviour
{
    public int itemHealth = 10;
    public float playerTriggerRange = 1f;
    public bool playerNearItem = false;
    public LayerMask playerLayer;
    
    private Animator myAnimator;

    // Update is called once per frame
    void FixedUpdate()
    {
        myAnimator = GetComponent<Animator>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNearItem = true;
            myAnimator.SetTrigger("Bounce");
        }
    }
}
