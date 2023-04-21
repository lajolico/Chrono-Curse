using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecoverHealth_Extra : MonoBehaviour
{

    public GameObject player;

    public GameObject button;
    public UnityEngine.UI.Button btn;
    
    public HealthBarScript healthBar;
    public int health = 100;
    // Start is called before the first frame update
    // private bool myBool;
    
    void Start()
    {
        btn = button.GetComponent<Button>();
    }

    void FixedUpdate()
    {
        btn.onClick.AddListener(TestButton);
    }

    void TestButton()
    {
        Debug.Log("Clicked!");
    }
}
