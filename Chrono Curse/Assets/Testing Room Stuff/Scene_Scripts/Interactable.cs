using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{

    public bool isInRange = false;
    public KeyCode interactKey;
    public UnityEvent interactAction;

    public PlayerControls PlayerControl;
    public InputAction interact;

    public bool isOpen = false;
    public DoorCollider passage;
    public DoorState door;
    // public TextController text;
    
    private void Awake()
    {
        GameObject gameObject = new GameObject();
        door = FindObjectOfType<DoorState>();
        // text = FindObjectOfType<TextController>();
        PlayerControl = new PlayerControls();
    }

    private void OnEnable()
    {
        interact = PlayerControl.Land.Interact;
        interact.Enable();
    }
    private void OnDisable()
    {
        interact.Disable();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange)
        {
            
            // text.playerNearObject(isInRange, isOpen);

            if (interact.triggered)
            {
                if (isOpen)
                {
                    isOpen = false;
                    door.ChangeSprite(isOpen);
                    passage.allowPassage();
                }
                else
                {
                    isOpen = true;
                    door.ChangeSprite(isOpen);
                    passage.allowPassage();
                }     
            }
        }
        else
        {
            // text.playerNearObject(false, isOpen);
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            Debug.Log("Player is now in range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            Debug.Log("Player is not in range");
        }
    }
}
