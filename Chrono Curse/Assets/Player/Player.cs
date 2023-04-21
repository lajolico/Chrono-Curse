using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // * Player movement components
    // * ==========================
    // Normal movement
    Vector2 moveDirection = Vector2.zero;
    public Rigidbody2D rb;
    public float moveSpeed = 0.2f; // Base player speed
    private float activeMoveSpeed = 0.2f; // Current player move speed at any given time
    public StaminaBarScript staminaBar;
    public float dashSpeed = 0.4f;
    // Idle animation handling
    public float idleAnimWait = .3f;
    private float idleAnimWaitCounter;
    // * ==========================
    // * Player controls
    // * ==========================
    public PlayerControls PlayerControl;
    // Individual actions controlled through inputs
    public InputAction move;
    public InputAction dash;
    public InputAction attack;
    public InputAction interact;
    // * ==========================
    // * Player attack, damage, and health components
    // * ==========================
    // Health mechanics
    public HealthBarScript healthBar;
    // Attack mechanics
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;
    public float nextAttackTime = 0f;
    public LayerMask enemyLayers;
    // * ==========================
    // * Animation control aspects
    // * ==========================
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    public GameObject playerEffects;
    private bool dashActive = false;

    public CanvasGroup youDiedScreen;
    public GameObject youDeathed;
    private bool fade = false;
    // * ==========================
    // * Player sound effects
    // * ==========================
    // public AudioSource audioSrc;

    // ! Initializes on player activation
    private void Awake()
    {
        PlayerControl = new PlayerControls();
        activeMoveSpeed = moveSpeed;
    }

    void Start()
    {
        healthBar.SetMaxHealth(PlayerManager.Instance.MaxHealth);
    }

    // ! Player controls enabled state
    private void OnEnable()
    {
        move = PlayerControl.Land.Move;
        move.Enable();
        dash = PlayerControl.Land.Dash;
        dash.Enable();
        attack = PlayerControl.Land.Attack;
        attack.Enable();
        interact = PlayerControl.Land.Interact;
        interact.Enable();
    }

    // ! Player controls disabled state
    private void OnDisable()
    {
        move.Disable();
        dash.Disable();
        attack.Disable();
        interact.Disable();
    }

    void Update()
    {
        // ? Triggers attack animation
        // ? ==========================
        if (Time.time >= nextAttackTime)
        {
            if(attack.triggered)
            {
                // FindObjectOfType<AudioManager>().Play("PlayerWalking");
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        if (dash.triggered)
        {
            staminaBar.GetComponent<StaminaBarScript>().UsingDash();
        }
    }

    // ! Sprite rendering components
    void FixedUpdate()
    {
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>(); // Used to flip sprite to change direction

        moveDirection = move.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveDirection.x * activeMoveSpeed, moveDirection.y * activeMoveSpeed);
        GetComponent<Player>().transform.Translate(rb.velocity * Time.deltaTime * activeMoveSpeed);

        // ? Fades in death scene
        if (fade)
        {
            youDiedScreen.alpha += Time.deltaTime;
            if (youDiedScreen.alpha >= 1)
            {
                fade = false;
                SceneManager.LoadScene(4);
            }
        }

        // ? ==========================
        // ? Handles sprite animation/render
        // ? ==========================
        // * Main animation components
        if (!dashActive)
        {
            myAnimator.SetInteger("Dashed", 0);
        }
        // =================================
        if (rb.velocity[0] > 0.01f) // Left
        {
            if (dashActive)
            {
                myAnimator.SetInteger("Dashed", 1);
            }
            else
            {
                myAnimator.SetInteger("Status", 1);
            }
            transform.localScale = new Vector2(1f, 1f);
        } 
        else if (rb.velocity[0] < -0.01f)
        {
            if (dashActive)
            {
                myAnimator.SetInteger("Dashed", 1);
            }
            else
            {
                myAnimator.SetInteger("Status", 1);
            }
            transform.localScale = new Vector2(-1f, 1f);
        }
        else if ((rb.velocity[1] < -0.01f) || (rb.velocity[1] > 0.01f))
        {
            if (dashActive)
            {
                myAnimator.SetInteger("Dashed", 1);
            }
            else
            {
                myAnimator.SetInteger("Status", 1);
            }
        }
        // ? ==========================
        // ? Changes animation from dash to idle in case player stops moving
        // ? ==========================
        else
        {
            myAnimator.SetInteger("Status", 0); 
            myAnimator.SetInteger("Dashed", 0);
        }
        // ? ==========================
    }

    // ! Called from StaminaBarScript to reset speed and player animation
    public void DashStatus(bool status)
    {
        dashActive = status;
        if (!dashActive)
        {
            activeMoveSpeed = moveSpeed;
        }
        else
        {
            activeMoveSpeed = dashSpeed;
        }
        
    }

    // ! Does attacks for player
    void Attack()
    {
        myAnimator.SetTrigger("Brattack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        Enemy enemy;
        foreach(Collider2D enemyCollider in hitEnemies)
        {
            enemy = enemyCollider.GetComponent<Enemy>();
            if(enemy != null) 
            {
                enemy.TakeDamage(PlayerManager.Instance.attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        myAnimator.SetTrigger("Damaged");
        PlayerManager.Instance.DamagePlayer(damage);
        healthBar.SetHealth(PlayerManager.Instance.Health);

        if(PlayerManager.Instance.Health <= 0)
        {
            youDeathed.SetActive(true);
            fade = true;
        }

    }

    public void RecoverHealth(int health)
    {
        myAnimator.SetTrigger("Healing");
        PlayerManager.Instance.HealPlayer(health);
        healthBar.SetHealth(PlayerManager.Instance.Health);
    }

    // ! Shows hit box area for player attack
    void OnDrawGizmosSelected() 
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
