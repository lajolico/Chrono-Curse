using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    public Slider slider;

    public GameObject healthBar;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        // Debug.Log("Bruh: " + health);
    }

    public void GETFUCKED()
    {
        // healthBar.SetActive(false);
        Debug.Log("Insane");
    }
}
