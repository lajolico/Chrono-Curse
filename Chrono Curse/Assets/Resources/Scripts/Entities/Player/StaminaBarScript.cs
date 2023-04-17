using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarScript : MonoBehaviour
{

    public GameObject myPlayer;
    public GameObject dashEffect;
    // Dash mechanic
    public float dashUsage = 0.5f;
    public bool dashAvailable = true;
    // Stamina for player to use dash
    private float currentStamina;
    public float maxPlayerStamina = 100;

    public bool usingDash = false;
    private bool dashActive = false;
    private bool rechargeDash = false;

    // Slider bar used to keep track of stamina and set ui bar for stamina
    public Slider slider;

    void Start()
    {
        currentStamina = maxPlayerStamina;
        SetMaxStamina(maxPlayerStamina); 
    }

    void FixedUpdate()
    {
        if (usingDash)
        {
            // ? ==========================
            // ? Triggers dash animation and effect
            // ? ==========================
            if (currentStamina == maxPlayerStamina)
            {
                if (dashAvailable)
                {
                    dashActive = true; // Plays dash animation
                    dashAvailable = false; // Bool to see if dash can be used
                    myPlayer.GetComponent<Player>().DashStatus(dashActive);
                }
            }

            if (currentStamina > 0f && dashActive)
            {
                UseStamina(dashUsage);
            }
            else if (currentStamina <= 0f) // Makes sure that the current stamina is at 0 before recharging
            {
                currentStamina = 0f;
                rechargeDash = true;
                dashActive = false;
                usingDash = false;
                myPlayer.GetComponent<Player>().DashStatus(dashActive); // Turns off dash animation and resets speed
            }
        }
        else if (rechargeDash)
        {
            RechargeStamina(dashUsage);
            if (currentStamina >= maxPlayerStamina)
            {
                currentStamina = maxPlayerStamina;
                SetStamina(currentStamina);
                dashAvailable = true;
                rechargeDash = false;
                dashActive = false;
            }
        }
        
    }

    public void UsingDash()
    {
        if (!rechargeDash)
        {
            usingDash = true;
            PlayDashEffect();
        }
        
    }

    void PlayDashEffect()
    {
        dashEffect.GetComponent<PlayerEffects>().DashingEffect();
    }

    void SetMaxStamina(float stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    void SetStamina(float stamina)
    {
        slider.value = stamina;
    }

    void UseStamina(float stamina)
    {
        currentStamina -= stamina;
        SetStamina(currentStamina);
    }

    void RechargeStamina(float stamina)
    {
        currentStamina += stamina;
        SetStamina(currentStamina);
    }
}
