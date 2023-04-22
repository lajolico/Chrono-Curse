using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public LayerMask playerLayer;
    private float detectionRange;
    public GameObject enemyController;

    void Start()
    {
        var circle = gameObject.GetComponent<CircleCollider2D>();
        circle.radius = detectionRange;
        circle.offset = new Vector2(0f, -0.5f);
    }

    public void SetRange(float range)
    {
        detectionRange = range;
    }

    // Start is called before the first frame update
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyController.GetComponent<Enemy>().PlayerInRange(true);
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyController.GetComponent<Enemy>().PlayerInRange(false);
        }
    }
}
