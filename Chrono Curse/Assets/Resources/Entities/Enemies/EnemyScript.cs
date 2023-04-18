using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public Transform player;

    public GameObject smallPig;
    public Transform castPoint;


    public float speed = 200f;
    public float agroRange = 10f;

    Rigidbody2D rb;

    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;

    bool isFacingLeft = false;
    bool isAgro = false;
    bool isSearching = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mySpriteRenderer = smallPig.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        CanSeePlayer(distanceToPlayer);

        if (CanSeePlayer(distanceToPlayer))
        {
            isAgro = true;
        }
        else
        {
            if (!isSearching)
            {
                isSearching = true;
                Invoke("StopChasingPlayer", 1);
            }
            
        }

        if (isAgro)
        {
            ChasePlayer();
        }

    }

    bool CanSeePlayer(float distance)
    {
        bool val = false;
        float castDist = distance;

        if (isFacingLeft)
        {
            castDist = -distance;
        }

        Vector2 endPos = castPoint.position + Vector3.right * castDist;

        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, 1 << LayerMask.NameToLayer("Player"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                val = true;
            }
            else
            {
                val = false;
            }

            Debug.DrawLine(castPoint.position, hit.point, Color.yellow);
        }
        else
        {
            Debug.DrawLine(castPoint.position, endPos, Color.blue);
        }
        return val;
    }


    void ChasePlayer()
    {
        if(transform.position.x < player.position.x)
        {
            rb.velocity = new Vector2(speed, 0);
            transform.localScale = new Vector2(1, 1);
            isFacingLeft = false;
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
            transform.localScale = new Vector2(-1, 1);
            isFacingLeft = true;
        }
    }

    void StopChasingPlayer()
    {
        isSearching = false;
        isAgro = false;
        rb.velocity = new Vector2(0, 0);
    }
}
