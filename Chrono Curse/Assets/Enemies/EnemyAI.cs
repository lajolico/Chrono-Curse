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
    public bool isInRange = false; // Player in range?

    public float detectionRange = 10f; // Allows setting of player detection range
    public float detectionCircleOffsetX = -.5f; // Allows setting of offset to make detection range centered on enemy

    Seeker seeker;
    Rigidbody2D rb;

    public Transform PigSmall;

    // * Stuff for attacking player
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;
    public float nextAttackTime = 0f;
    public LayerMask playerLayer;
    public bool attackPlayer = false;
    public bool allowedToAttack = false;
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
        var circle = gameObject.GetComponent<CircleCollider2D>();
        circle.radius = detectionRange;
        circle.offset = new Vector2(0f, detectionCircleOffsetX);

        // var attackSquare = smallPig.GetComponent<BoxCollider2D>();

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

        if (Time.time >= nextAttackTime)
        {        
            Debug.Log("Time: " + Time.time + "nextAttackTime: " + nextAttackTime);
            allowedToAttack = true;
        }

        if (!attackPlayer)
        {

            if (isInRange)
            {

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
                    // EnemyAnimationController(1); // Run Anim
                    PigSmall.localScale = new Vector3(-1f, 1f, 1f);
                }
                else if (rb.velocity[0] <= 1f) // Right
                {
                    // EnemyAnimationController(1);// Run Anim
                    PigSmall.localScale = new Vector3(1f, 1f, 1f);
                }
                else // Idle
                {
                    // EnemyAnimationController(0);
                }
            }
            else
            {
                // Debug.Log("why we stopping...");
                // StopChasingPlayer();
            }
        }
        // else if (attackPlayer && allowedToAttack)
        // {
        //     StopChasingPlayer();
        //     Attack();
        // }
        
    }






    // void StopChasingPlayer()
    // {
    //     EnemyAnimationController(0);
    //     rb.velocity = new Vector2(0, 0);
    // }

    // void OnTriggerStay2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         isInRange = true;
    //         // Debug.Log("Player is a rat bitch");
    //     }
    // }
    
    // void OnTriggerExit2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         isInRange = false;
    //         // Debug.Log("Player is not in range of enemy");
    //     }
    // }

    // public void AttackingPlayer(bool attack)
    // {
    //     attackPlayer = attack;
    // }

    // void Attack()
    // {
    //     // myAnimator.SetTrigger("Brattack");
    //     Collider2D[] hitplayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
    //     foreach(Collider2D person in hitplayer)
    //     {
    //         // Debug.Log("We hit " + person.name);
    //         person.GetComponent<Player>().TakeDamage(attackDamage);
    //     }
    //     myAnimator.SetTrigger("Attack");
    //     // Debug.Log(isInRange);
    //     attackPlayer = false;
    //     allowedToAttack = false;
    //     nextAttackTime = Time.time + 1f / attackRate;
    // }

    // public void TakeDamage(int damage)
    // {
    //     currentHealth -= damage;
    //     myAnimator.SetTrigger("Hurt");
    //     if (currentHealth <= 0)
    //     {
    //         Death();
    //     }
    // }

    // public void EnemyAnimationController(int animStatus) // Sets run or idle animation
    // {
    //     if (animStatus == 1)
    //     {
    //         myAnimator.SetInteger("Status", 1);
    //     }
    //     else
    //     {
    //         myAnimator.SetInteger("Status", 0);
    //     }

    // }

    // void Death()
    // {
    //     myAnimator.SetTrigger("Death");
    //     // Debug.Log("Deadeded");

    //     GetComponent<Collider2D>().enabled = false;
    // }

    // void Die()
    // {
    //     Destroy(mySpriteRenderer);
    //     // mySpriteRenderer.enabled = false;
    // }
}
