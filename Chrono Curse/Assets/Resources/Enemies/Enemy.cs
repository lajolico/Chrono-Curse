using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class Enemy : MonoBehaviour
{
    private EnemyObject enemyObject;

    private Transform target;

    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }

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
    private EnemyType enemyType;
    public int baseAttackDamage = 10;

    public GameObject circleDetector;

    /// <summary>
    /// Attack Checker for our EnemyAI
    /// </summary>
    private Animator mainEnemyAnimator, attackCheckerAnimator;

    internal void DamagePlayer()
    {
        PlayerManager.Instance.DamagePlayer(this.attackDamage);
    }

    private SpriteRenderer attackCheckerRenderer;

    public void SetPropertiesFromObjectData(EnemyObject enemyObjData, 
        Animator _enemyAnimator, GameObject _attackCheckerGFX)
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
        this.attackRange = enemyObjData.attackRange;
        this.attackRate = 1 / enemyObjData.attackRate;
        this.nextAttackTime = enemyObjData.nextAttackTime;
        this.enemyType = enemyObjData.enemyType;
        enemyObject = enemyObjData;
    }

    public void SetPropertiesFromState(EnemyState state, Animator _enemyAnimator, GameObject _attackCheckerGFX)
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
        this.attackRate = 1 / state.attackRate;
        this.nextAttackTime = state.nextAttackTime;
        this.enemyType = state.enemyType;
    }


    public void SetAttackDamage(int playerLevel)
    {
        // Calculate the enemy's attack damage based on the player's level
        float damageMultiplier = 1.0f + ((float)playerLevel / 10.0f); // Increase damage by 10% for every player level
        attackDamage = Mathf.RoundToInt(baseAttackDamage * damageMultiplier);
    }


    public EnemyObject GetEnemyObjectData()
    {
        return enemyObject;
    }

    void Awake()
    {
        seeker = GetComponent<Seeker>();
    }

    private void Start()
    {
        circleDetector.GetComponent<PlayerDetector>().SetRange(detectionRange);

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
            StopChasingPlayer();
            Attacking();
        }
        
    }

    void StopChasingPlayer()
    {
        EnemyAnimationController(0);
        rb.velocity = new Vector2(0, 0);
    }

    public void PlayerInRange(bool inRange)
    {
        isInRange = inRange;
    }

    public void AttackingPlayer(bool attack)
    {
        attackPlayer = attack;
    }

    void Attacking()
    {
        attackCheckerAnimator.SetTrigger("Attack");
        allowedToAttack = false;
        nextAttackTime = Time.time + 1f / attackRate;
    }

    public void DoPlayerDamage()
    {
        Collider2D[] hitplayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach(Collider2D player in hitplayer)
        {
            player.GetComponent<Player>().TakeDamage(attackDamage);
            StartCoroutine(FloatingTextManager.Instance.ShowFloatingText("-"+attackDamage.ToString(), 
            PlayerManager.Instance.GetPlayerPosition(), 0.5f, FloatingTextType.DamagePlayer));
            CinemachineShake.Instance.ShakeCamera();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        attackCheckerAnimator.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FloatingTextManager.Instance.ShowFloatingText("-"+damage.ToString(), transform.position, 0.3f, FloatingTextType.DamageEnemy));
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
        attackCheckerAnimator.SetTrigger("Death");
    }

    public void DestroyEnemy()
    {
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;
        attackCheckerRenderer.enabled = false;
        AddToPlayer(enemyType);
        if(PlayerManager.Instance.isPlayerInDungeon == true)
        {
            EnemyManager.Instance.RemoveEnemy(this.gameObject);

        }else
        {
           DemoEnemyManager.Instance.RemoveEnemy(this.gameObject);
        }

         Destroy(this.gameObject);
    }

    public void AddToPlayer(EnemyType type)
    {
        int xpToAdd = 0;
        switch (type)
        {
            case EnemyType.Normal:
                xpToAdd = 50;
                break;
            case EnemyType.Elite:
                xpToAdd = 100;
                break;
            case EnemyType.Boss:
                xpToAdd = 500;
                break;
        }
        PlayerManager.Instance.AddXP(xpToAdd);
        PlayerManager.Instance.AddKill(1);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(int health)
    {
        this.maxHealth = health;
    }

    public int GetAttackDamage()
    {
        return this.attackDamage;
    }
}
