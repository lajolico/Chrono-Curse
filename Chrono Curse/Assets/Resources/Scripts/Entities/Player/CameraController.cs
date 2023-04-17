using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;

    private CinemachineVirtualCamera vCamera;


    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraController>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        vCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        vCamera.LookAt  = PlayerManager.Instance.GetPlayerTransform();
        vCamera.Follow = PlayerManager.Instance.GetPlayerTransform();
    }
}
