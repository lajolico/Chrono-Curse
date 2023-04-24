using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    Vector2 moveDirection = Vector2.zero;
    public Rigidbody2D rb;
    public float moveSpeed = 0.2f; // Base player speed
    private float activeMoveSpeed = 0.2f; // Current player move speed at any given time
    public float dashSpeed = 0.4f;
    public StaminaBarScript staminaBar;

    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    public GameObject playerEffects;
    private bool dashActive = false;

    public HealthBarScript healthBar;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    public int attackDamage = 40;
    public float nextAttackTime = 0f;

    public CanvasGroup youDiedScreen;
    public GameObject youDeathed;
    private bool fade = false;

    public PlayerAudio audioForPlayer;
    public float footStepWalkSpeed;
    public float dashStepWalkSpeed = 0.15f;

    public LayerMask enemyLayers;
    public LayerMask itemLayers;

    public PlayerControls PlayerControl;

    public InputAction move;
    public InputAction dash;
    public InputAction attack;
    public InputAction interact;

    // ! Initializes on player activation
    private void Awake()
    {
        PlayerControl = new PlayerControls();
        activeMoveSpeed = moveSpeed;
        healthBar.SetMaxHealth(PlayerManager.Instance.MaxHealth);
        footStepWalkSpeed = audioForPlayer.returnStepSpeed();
    }

    // void Start()
    // {
    //     healthBar.SetMaxHealth(PlayerManager.Instance.MaxHealth);
    // }

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

        // Debug.Log(move.ReadValue<Vector2>());
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
                GameManager.Instance.PlayerDied();
            }
        }

        // ! Plays footstep audio
        if ((rb.velocity[0] > 0.01f) || (rb.velocity[1] > 0.01f) || (rb.velocity[0] < -0.01f) || (rb.velocity[1] < -0.01f))
        {
            audioForPlayer.PlayerMovementStatus(true);
        }
        else
        {
            audioForPlayer.PlayerMovementStatus(false);
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
            audioForPlayer.setStepSpeed(footStepWalkSpeed);
        }
        else
        {
            activeMoveSpeed = dashSpeed;
            audioForPlayer.setStepSpeed(dashStepWalkSpeed);
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

        Collider2D[] hitProps = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, itemLayers);
        Crate crate;
        Barrel barrel;
        Pot pot;
        foreach(Collider2D propCollider in hitProps)
        {
            crate = propCollider.GetComponent<Crate>();
            barrel = propCollider.GetComponent<Barrel>();
            pot = propCollider.GetComponent<Pot>();
            if(crate != null) 
            {
                crate.TakeDamage(attackDamage);
                // enemy.TakeDamage(PlayerManager.Instance.attackDamage);
            }
            if(barrel != null)
            {
                barrel.TakeDamage(attackDamage);
            }
            if(pot != null)
            {
                pot.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        myAnimator.SetTrigger("Damaged");
        PlayerManager.Instance.DamagePlayer(damage);
        healthBar.SetHealth(PlayerManager.Instance.Health);
        Debug.Log("Player health: " + PlayerManager.Instance.Health);
        if(PlayerManager.Instance.Health <= 0)
        {
            youDeathed.SetActive(true);
            fade = true;
        }

    }

    public void RecoverHealth()
    {
        PlayerManager.Instance.HealPlayer();
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
