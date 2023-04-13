using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LootPickup : MonoBehaviour
{
    private Loot loot;
    public float HoverDistance = 0.1f;
    public float HoverSpeed = 1f;
    private bool isHovering = false;

    private Vector2 originalPosition;
    private CircleCollider2D triggerCollider;

    private void Start()
    {
        // Get the CircleCollider2D component of the loot object
        triggerCollider = GetComponent<CircleCollider2D>();
 

        // Calculate the radius based on the size of the sprite for our player to pick it up
        float radius = Mathf.Max(transform.localScale.x, transform.localScale.y) / 2f;

        // Set the radius of the trigger collider
        triggerCollider.radius = radius;

    }

    private void OnEnable()
    {
        originalPosition = transform.position;
        isHovering = true;
        StartCoroutine(Hover());
    }

    private void OnDisable()
    {
        isHovering = false;
        transform.position = originalPosition;
        StopAllCoroutines();
    }

    private IEnumerator Hover()
    {
        while (isHovering)
        {
            float newY = Mathf.Sin(Time.time * HoverSpeed) * HoverDistance;
            transform.position = originalPosition + new Vector2(0f, newY);
            yield return null;
        }
    }

    public void SetLoot(Loot newLoot)
    {
        loot = newLoot;
        GetComponent<SpriteRenderer>().sprite = newLoot.Sprite;
    }

    public Loot Loot
    {
        get { return loot; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LootManager.Instance.CollectLoot(this);
        }
    }
}
