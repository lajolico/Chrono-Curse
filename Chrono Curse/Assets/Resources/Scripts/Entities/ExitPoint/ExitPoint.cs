using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{

    public static ExitPoint Instance { get; private set; }

    [SerializeField]
    public GameObject exitPointPrefab;

    //Ensure that our player persists between scene changes
    private GameObject exitPointInstance;

    private ExitPoint() { }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // If another instance of the class already exists, destroy this one
            Destroy(gameObject);
        }
    }

    public void SpawnExitPoint()
    {
        if (exitPointInstance == null)
        {
            exitPointInstance = Instantiate(exitPointPrefab);
        }
        else
        {
            Debug.LogWarning("Exit Point instance already exists.");
        }
    }

    public void DestroyExit()
    {
        if (exitPointInstance != null)
        {
            Destroy(exitPointInstance);
            exitPointInstance = null;
        }
    }

    // Public method to get the player's position
    public void SetExitPosition(Vector3 newPosition)
    {
        if (exitPointInstance != null)
        {
            exitPointInstance.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Exit instance does not exist in SetExitPosition()");
        }
    }

    public Vector3 GetExitPosition()
    {
        if(exitPointInstance != null)
        {
            return exitPointInstance.transform.position;
        }else
        {
            Debug.LogWarning("Exit Point Instance is null in GetExitPosition()");
            return Vector3.zero; 
        }
    }

    public ExitPointData GetExitPointData()
    {
        ExitPointData exitPointData = new ExitPointData();
        exitPointData.position = GetExitPosition();

        return exitPointData;
    }

    internal void LoadExitPoint(ExitPointData exitPointData)
    {
        SpawnExitPoint();
        SetExitPosition(exitPointData.position);
    }
}
