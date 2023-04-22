using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBridge : MonoBehaviour
{

    public GameObject animationBridge;

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    public void EnemyHasDied()
    {
        animationBridge.GetComponent<Enemy>().DestroyEnemy();
    }
    
}
