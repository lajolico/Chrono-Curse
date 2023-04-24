using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chesty : MonoBehaviour
{
    public int itemHealth = 10;
    public float playerTriggerRange = 1f;
    public bool playerNearItem = false;
    public LayerMask playerLayer;
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    public PlayerManager putt;
    private Animator myAnimator;

    void Start()
    {
        putt.SetPlayerPosition(new Vector3(-0.5f, -1f, 0f));
    }

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
