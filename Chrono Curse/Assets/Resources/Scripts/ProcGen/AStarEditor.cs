using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Analytics;
using System.Drawing;

public class AStarEditor : MonoBehaviour
{

    public static AStarEditor Instance { get; private set; }

    [SerializeField]
    private int offset = 25;

    // Get the AstarData instance
    private AstarData data;

    private void Awake()
    {
        if(Instance== null || Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

       data = AstarPath.active.data;
    }
    public void ResizeGraph(int width, int height, List<Vector2Int> roomCenters)
    {
        // Get the grid graph
        GridGraph gridGraph = data.gridGraph;

        Vector2Int centerPosition = Vector2Int.zero;

        // Calculate the average of all room centers
        foreach (Vector2Int roomCenter in roomCenters)
        {
            centerPosition += roomCenter;
        }
        centerPosition /= roomCenters.Count;

        // Set the new size of the graph
        gridGraph.SetDimensions(width*2, height*2, gridGraph.nodeSize);

        // Center the graph at the specified position
        gridGraph.center = (Vector3)(Vector3Int)centerPosition + new Vector3(0, 1.5f, 0);

        // Update the graph
        AstarPath.active.Scan();
    }
}


