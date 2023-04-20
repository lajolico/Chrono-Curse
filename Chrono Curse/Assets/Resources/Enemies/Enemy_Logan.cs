using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class Enemy_Logan : MonoBehaviour
{
    private EnemyObject enemyObject;

    private Transform target;

    /// <summary>
    ///AI
    /// </summary>
    /// 
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    public bool isInRange = false; // Player in range?
    public float detectionRange = 10f; // Allows setting of player detection range
    Seeker seeker;
    Rigidbody2D rb;
    public float nextWaypointDistance = 3;

    // * Stuff for attacking player
    public Transform enemyTransform;
    public Transform attackPoint;
    private float attackRange;
    private int attackDamage;
    private float attackRate;
    private float nextAttackTime;
    private bool attackPlayer = false;
    private bool allowedToAttack = false;
    private int maxHealth;
    public LayerMask playerLayer;
    private int currentHealth;
    private float speed;


    /// <summary>
    /// Attack Checker for our EnemyAI
    /// </summary>
    private Animator mainEnemyAnimator, attackCheckerAnimator;
    private SpriteRenderer attackCheckerRenderer;

    public void SetPropertiesFromObjectData(EnemyObject enemyObjData, 
        Animator _enemyAnimator, SpriteRenderer _spriteRenderer, GameObject _attackCheckerGFX)
    {
        mainEnemyAnimator = _enemyAnimator.GetComponent<Animator>();
        attackCheckerAnimator = _attackCheckerGFX.GetComponent<Animator>();
        attackCheckerRenderer = _attackCheckerGFX.GetComponent<SpriteRenderer>();

        // Set the properties of the enemy components
        this.attackCheckerAnimator.runtimeAnimatorController = enemyObjData.animatorController;
        this.mainEnemyAnimator.runtimeAnimatorController = enemyObjData.animatorController;
        this.attackCheckerRenderer.sprite = enemyObjData.sprite;

        this.speed = enemyObjData.speed;
        this.maxHealth = enemyObjData.health;
        currentHealth = this.maxHealth;
        this.attackDamage = enemyObjData.attackDamage;
        this.attackRange = enemyObjData.attackRange;
        this.attackRate = enemyObjData.attackRate;
        this.nextAttackTime = enemyObjData.nextAttackTime;
        enemyObject = enemyObjData;
    }

    public void SetPropertiesFromState(EnemyState state, Animator _enemyAnimator, SpriteRenderer _spriteRenderer, GameObject _attackCheckerGFX)
    {
        mainEnemyAnimator = _enemyAnimator.GetComponent<Animator>();
        attackCheckerAnimator = _attackCheckerGFX.GetComponent<Animator>();
        attackCheckerRenderer = _attackCheckerGFX.GetComponent<SpriteRenderer>();

        this.attackCheckerAnimator.runtimeAnimatorController = state.animatorController;
        this.mainEnemyAnimator.runtimeAnimatorController = state.animatorController;
        this.attackCheckerRenderer.sprite = state.sprite;

        this.speed = state.speed;
        this.maxHealth = state.health;
        currentHealth = this.maxHealth;
        this.attackDamage = state.attackDamage;
        this.attackRange = state.attackRange;
        this.attackRate = state.attackRate;
        this.nextAttackTime = state.nextAttackTime;
    }

    public EnemyObject GetEnemyObjectData()
    {
        return enemyObject;
    }

    private void Start()
    {
        var circle = gameObject.GetComponent<CircleCollider2D>();
        circle.radius = detectionRange;
        circle.offset = new Vector2(0f, -0.5f);

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        target = PlayerManager.Instance.GetPlayerTransform();

        InvokeRepeating("UpdatePath",0f, .5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
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
                    EnemyAnimationController(1); // Run Anim
                    enemyTransform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else if (rb.velocity[0] <= 1f) // Right
                {
                    EnemyAnimationController(1);// Run Anim
                    enemyTransform.localScale = new Vector3(1f, 1f, 1f);
                }
                else // Idle
                {
                    EnemyAnimationController(0);
                }
            }
            else
            {
                 StopChasingPlayer();
            }
        }
        else if (attackPlayer && allowedToAttack)
        {
            Debug.Log("Attacking");
            StopChasingPlayer();
            Attack();
        }
        
    }

    void StopChasingPlayer()
    {
        EnemyAnimationController(0);
        rb.velocity = new Vector2(0, 0);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    public void AttackingPlayer(bool attack)
    {
        attackPlayer = attack;
    }

    void Attack()
    {
        Collider2D[] hitplayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach(Collider2D player in hitplayer)
        {
            player.GetComponent<Player>().TakeDamage(attackDamage);
            Debug.Log("Player Hit!");
            StartCoroutine(FloatingTextManager.Instance.ShowFloatingText(attackDamage.ToString(), PlayerManager.Instance.GetPlayerPosition(), 0.6f));
        }
        attackCheckerAnimator.SetTrigger("Attack");
        attackPlayer = false;
        allowedToAttack = false;
        nextAttackTime = Time.time + 1f / attackRate;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        attackCheckerAnimator.SetTrigger("Hurt");
        StartCoroutine(FloatingTextManager.Instance.ShowFloatingText(damage.ToString(), gameObject.transform.position, 0.6f));
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void EnemyAnimationController(int animStatus) // Sets run or idle animation
    {
        if (animStatus == 1)
        {
            attackCheckerAnimator.SetInteger("Status", 1);
        }
        else
        {
            attackCheckerAnimator.SetInteger("Status", 0);
        }
    }

    private void Die()
    {
        PlayerManager.Instance.AddKill(1);
        EnemyManager.Instance.RemoveEnemy(this.gameObject);
        attackCheckerAnimator.SetTrigger("Death");
        gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        Destroy(this.gameObject, 1.0f);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(int health)
    {
        this.maxHealth = health;
    }

}
