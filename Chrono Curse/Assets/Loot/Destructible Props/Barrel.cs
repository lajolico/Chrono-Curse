using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public int itemHealth = 240;
    public LayerMask playerLayer;
    private Animator myAnimator;

    void FixedUpdate()
    {
        myAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            myAnimator.SetTrigger("Bounce");
        }
    }

    public void TakeDamage(int damage)
    {
        itemHealth -= damage;
        if (itemHealth <= 0)
        {
            myAnimator.SetTrigger("Break");
        }
        else
        {
            myAnimator.SetTrigger("Bounce");
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
