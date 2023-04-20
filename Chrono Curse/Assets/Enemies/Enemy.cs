using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
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
    public int currentHealth;

    public GameObject AttackCheckerGFX;

    public Animator myAnimator;
    public SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        mySpriteRenderer = AttackCheckerGFX.GetComponent<SpriteRenderer>();
        myAnimator = AttackCheckerGFX.GetComponent<Animator>();
    }

    void Start()
    {
        var circle = gameObject.GetComponent<CircleCollider2D>();
        circle.radius = detectionRange;
        circle.offset = new Vector2(0f, detectionCircleOffsetX);

        // var attackSquare = smallPig.GetComponent<BoxCollider2D>();
        //target = PlayerManager.Instance.GetPlayerTransform();

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
        if (Time.time >= nextAttackTime)
        {        
            // Debug.Log("Time: " + Time.time + "nextAttackTime: " + nextAttackTime);
            allowedToAttack = true;
        }

        if (!attackPlayer)
        {
            if (isInRange)
            {
                if (path == null) // Checks to see if there is a path for the enemy to follow
                {
                    return;

                }

                if (currentWaypoint >= path.vectorPath.Count) // Checks to see if the end of the path has been reached (i.e. if there is a player to chase)
                {
                    reachedEndOfPath = true;
                    return; // Breaks loop so that enemy stops trying to path
                }
                else
                {
                    reachedEndOfPath = false;
                }

                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position); // Enemy pathing in correct direction
                Vector2 force = direction * speed; // Sets enemy's speed

                rb.velocity = new Vector2(direction.x * speed, direction.y * speed); 

                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]); // Distance between waypoints

                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }

                if (rb.velocity[0] >= 1f) // Left
                {
                    EnemyAnimationController(1); // Run Anim
                    PigSmall.localScale = new Vector3(-1f, 1f, 1f); // Flips GameObject in correct direction
                }
                else if (rb.velocity[0] <= 1f) // Right
                {
                    EnemyAnimationController(1);// Run Anim
                    PigSmall.localScale = new Vector3(1f, 1f, 1f); // Flips GameObject in correct direction
                }
                else // Idle
                {
                    EnemyAnimationController(0);
                }
            }
            else // Puts enemy in idle animation and stops enemy movement
            {
                 StopChasingPlayer();
            }
        }
        else if (attackPlayer && allowedToAttack) // Enemy attacks player and stops moving if player is within range and enemy is allowed to attack
        {
            StopChasingPlayer();
            Attack();
        }
        
    }

    void StopChasingPlayer() // Stops enemy and changes animation to idle if enemy doesn't need to move
    {
        EnemyAnimationController(0);
        rb.velocity = new Vector2(0, 0);
    }

    void OnTriggerStay2D(Collider2D collision) // Checks to see if player is in the detection range of the enemy
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D collision) // Checks if the player is not in the detection range of the enemy
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    public void AttackingPlayer(bool attack) // Function called from AttackChecker to tell enemy that the player is close enough to attack
    {
        attackPlayer = attack;
    }

    void Attack() // PLays attack animation and changes enemy logic to wait for next available attack
    {
        myAnimator.SetTrigger("Attack"); // Plays attack animation
        allowedToAttack = false;
        nextAttackTime = Time.time + 1f / attackRate; // Sets amount of time enemy has to wait between attacks
    }

    public void DoPlayerDamage() // Attacks player that is in range and deals damage accordingly
    {
        Collider2D[] hitplayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach(Collider2D player in hitplayer)
        {
            player.GetComponent<Player>().TakeDamage(attackDamage); // Player that is within collider is dealt damage
        }
    }

    public void TakeDamage(int damage) // Enemy takes damage from player
    {
        currentHealth -= damage;
        myAnimator.SetTrigger("Hurt"); // Plays hurt animation
        if (currentHealth <= 0) // Enemy dies if their health falls low enough
        {
            DestroyEnemy();
        }
    }

    public void EnemyAnimationController(int animStatus) // Sets run or idle animation
    {
        if (animStatus == 1)
        {
            myAnimator.SetInteger("Status", 1); // Run
        }
        else
        {
            myAnimator.SetInteger("Status", 0); // Idle
        }
    }

    void DestroyEnemy() // Gets rid of enemy GameObject and collider + plays death animation
    {
        myAnimator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        Destroy(this.gameObject, 1.0f);
    }
}
