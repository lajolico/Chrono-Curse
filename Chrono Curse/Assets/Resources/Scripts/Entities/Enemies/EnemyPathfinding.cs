using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    public Transform target; // The target to chase
    public float moveSpeed = 5f; // The speed at which the enemy moves
    public float stoppingDistance = 1f; // The distance at which the enemy stops moving

    private Pathfinding pathfinding; // Reference to the Pathfinding script
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2[] path; // The calculated path to the target
    private int currentWaypoint = 0; // Index of the current waypoint in the path

    void Start()
    {
        pathfinding = GetComponent<Pathfinding>(); // Get the Pathfinding component
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void Update()
    {
        if (target == null) return; // If there's no target, return

        // Calculate the path to the target
        path = pathfinding.FindPath(transform.position, target.position);

        if (path.Length > 0)
        {
            // Move towards the next waypoint in the path
            Vector2 nextWaypoint = path[currentWaypoint];
            Vector2 moveDirection = (nextWaypoint - (Vector2)transform.position).normalized;
            rb.velocity = moveDirection * moveSpeed;

            // Check if the enemy has reached the current waypoint
            float distance = Vector2.Distance(transform.position, nextWaypoint);
            if (distance < stoppingDistance)
            {
                currentWaypoint++;
                if (currentWaypoint >= path.Length)
                {
                    // Reached the end of the path, reset the current waypoint
                    currentWaypoint = 0;
                }
            }
        }
        else
        {
            // If there's no path, stop moving
            rb.velocity = Vector2.zero;
        }
    }
}
