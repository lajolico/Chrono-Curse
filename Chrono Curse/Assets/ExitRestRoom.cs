using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRestRoom : MonoBehaviour
{

    [SerializeField] private GameObject door;
    [SerializeField] private Sprite openDoor, closeDoor;
    [SerializeField] private GameObject infoText;
    private bool canEnter = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
            infoText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
            infoText.SetActive(false);
        }
    }

    private void Update()
    {
        if ( (canEnter && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (canEnter && Input.GetKeyDown(KeyCode.A)))
        {
            door.GetComponent<SpriteRenderer>().sprite = openDoor;
            GameManager.Instance.LoadDungeonScene(true);
        }
    }
}
