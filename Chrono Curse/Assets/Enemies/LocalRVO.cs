using UnityEngine;
using System.Collections;
using Pathfinding.RVO;
public class LocalRVO : MonoBehaviour {
    
    
    RVOController controller;
    // Use this for initialization
    void Awake () {
        controller = GetComponent<RVOController>();
    }
    // Update is called once per frame
    public void Update () {
        // Just some point far away
        var targetPoint = transform.position + transform.forward * 100;
        // Set the desired point to move towards using a desired speed of 10 and a max speed of 12
        controller.SetTarget(targetPoint, 10, 12);
        // Calculate how much to move during this frame
        // This information is based on movement commands from earlier frames
        // as local avoidance is calculated globally at regular intervals by the RVOSimulator component
        var delta = controller.CalculateMovementDelta(transform.position, Time.deltaTime);
        transform.position = transform.position + delta;
    }
}