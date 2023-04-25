using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    [SerializeField]private CinemachineImpulseSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance of the class already exists, destroy this one
            Destroy(gameObject);
        }
    }

    public void ShakeCamera()
    {
        Debug.Log("Shake!");
        source.GenerateImpulse();
    }
}
