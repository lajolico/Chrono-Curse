using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextController : MonoBehaviour
{
    public GameObject floatingText;
    public bool state, open;
    public TextMeshProUGUI textRendered;
    // private Animator myAnimator;

    private void Awake()
    {
        GameObject gameObject = new GameObject();
        textRendered = floatingText.GetComponent<TextMeshProUGUI>();
    }

    // void FixedUpdate()
    // {
    //     myAnimator = GetComponent<Animator>();
    // }

    public void playerNearObject(bool state, bool open)
    {
        if (state)
        {
            textRendered.enabled = true;
            if (open)
            {
                textRendered.text = "Close";
        //         // floatingText.SetActive(true);
        //         // myAnimator.SetInteger("InteractWithDoor", 1);
        //         GetComponent<TextMesh>().text = "Close";
        //         Debug.Log("Close status animation");
            }
            else
            {
        //         floatingText.SetActive(true);
                textRendered.text = "Open";
        //         // myAnimator.SetInteger("InteractWithDoor", 1);
        //         GetComponent<TextMesh>().text = "Open";
        //         Debug.Log("Open status animation");
            }
            
        }
        else
        {
            textRendered.enabled = false;
            // myAnimator.SetInteger("InteractWithDoor", 0);
            // Destroy(floatingText);
        }
    }
}
