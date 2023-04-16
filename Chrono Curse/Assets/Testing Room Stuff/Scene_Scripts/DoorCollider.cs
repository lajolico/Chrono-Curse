using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    public GameObject doorCollision;
    public Interactable state;
    // Start is called before the first frame update
    public void allowPassage()
    {
        if (state.isOpen)
        {
            doorCollision.GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            doorCollision.GetComponent<Collider2D>().enabled = true;
        }
        
    }
}
