using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    public Transform PigSmall;

    public int maxHealth = 100;
    int currentHealth;

    public GameObject smallPig;

    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        mySpriteRenderer = smallPig.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f, .5f);
        
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
        seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        myAnimator = smallPig.GetComponent<Animator>();
        mySpriteRenderer = smallPig.GetComponent<SpriteRenderer>(); // Used to make sprite disappear

        if (path == null)
        {
            return;

        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);
        Vector2 force = direction * speed;

        rb.velocity = new Vector2(direction.x * speed, direction.y * speed);

        

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (rb.velocity[0] >= 1f) // Left
        {
            EnemyAnimationController(1); // Run Anim
            PigSmall.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (rb.velocity[0] <= 1f) // Right
        {
            EnemyAnimationController(1);// Run Anim
            PigSmall.localScale = new Vector3(1f, 1f, 1f);
        }
        else // Idle
        {
            EnemyAnimationController(0);
        }
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        myAnimator.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void EnemyAnimationController(int animStatus)
    {
        if (animStatus == 1)
        {
            myAnimator.SetInteger("Status", 1);
        }
        else
        {
            myAnimator.SetInteger("Status", 0);
        }

    }

    void Death()
    {
        myAnimator.SetTrigger("Death");
        Debug.Log("Deadeded");

        GetComponent<Collider2D>().enabled = false;
    }

    void Die()
    {
        Destroy(mySpriteRenderer);
        // mySpriteRenderer.enabled = false;
    }
}
