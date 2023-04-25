using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoRecoverHealth : MonoBehaviour
{

    public GameObject player;
    public Player playerScript;
    // public GameObject button;
    // public UnityEngine.UI.Button btn;
    
    // public HealthBarScript healthBar;
    public int health = 100;
    // Start is called before the first frame update
    // private bool myBool;
    
    // void Start()
    // {
    //     btn = button.GetComponent<Button>();
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.RecoverHealth();
        }
    }
}
