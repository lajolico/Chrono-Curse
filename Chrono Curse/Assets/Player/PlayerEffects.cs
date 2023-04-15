using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    public GameObject playerEffects;
    private Animator effects;
    private SpriteRenderer mySpriteRenderer;

    private void awake()
    {
        mySpriteRenderer = playerEffects.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        effects = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>(); // Used to flip sprite to change direction
    }

    public void DashingEffect()
    {
        Debug.Log("Dashing effect");
        effects.SetTrigger("Dashing");
    }
}
