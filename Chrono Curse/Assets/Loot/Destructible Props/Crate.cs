using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public int itemHealth = 240;
    public PropAudio propAudio;
    public LayerMask playerLayer;
    private Animator myAnimator;
    bool playAudioAtCorrectMoment = false;

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

    public void PlayPropAudio()
    {
        if (itemHealth <= 0)
        {
            propAudio.TriggerToPlayAudio(false);
        }
        else if (playAudioAtCorrectMoment)
        {
            propAudio.TriggerToPlayAudio(true);
            playAudioAtCorrectMoment = false;
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
            playAudioAtCorrectMoment = true;
            myAnimator.SetTrigger("Bounce");
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
