using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.IO;


public class AStarEditor : MonoBehaviour
{
    public static AStarEditor Instance { get; private set; }

    // Get the AstarData instance
    private AstarData data;

    private static string graphSaveFile = "/graph.bytes";

    // Create a file path to save the graph data
    string loadFilePath;
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

        loadFilePath = Application.persistentDataPath + graphSaveFile;
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
        gridGraph.center = (Vector3Int)centerPosition;

        // Update the graph
        AstarPath.active.Scan();
    }

    /// <summary>
    /// Save our AStar graph.
    /// </summary>
    public void SaveGraphData()
    {
        Pathfinding.Serialization.SerializeSettings settings = new Pathfinding.Serialization.SerializeSettings();
        //Save node info, and output nice JSON
        settings.nodes = false;
        // Get the byte data of the graph
        byte[] graphBytes = data.SerializeGraphs(settings);


        // Write the graph data bytes to file
        File.WriteAllBytes(loadFilePath, graphBytes);

        Debug.Log("Graph data saved to file: " + loadFilePath);
    }

    public void LoadGraphData()
    {
        // Create a file path to load the graph data from
        string loadFilePath = Application.persistentDataPath + graphSaveFile;

        // Check if the file exists
        if (File.Exists(loadFilePath))
        {
            // Load the graph data bytes from file
            byte[] graphBytes = File.ReadAllBytes(loadFilePath);

            // Deserialize the graph data and set it as the A* graph data
            data.DeserializeGraphs(graphBytes);

            Debug.Log("Graph data loaded from file: " + loadFilePath);

            AstarPath.active.Scan();
        }
        else
        {
            Debug.LogError("Graph data file does not exist: " + loadFilePath);
        }
    }

    public void DeleteGraph()
    {
        if (File.Exists(loadFilePath))
        {
            File.Delete(Application.persistentDataPath + graphSaveFile);
        }
        else
        {
            Debug.LogError("Graph data file does not exist: " + loadFilePath);
        }
    }
}


