using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int poolSize = 10;

    [SerializeField]
    private bool canGrow = true;

    //Where are our pooled objects will go
    private List<GameObject> pool;

    /// <summary>
    /// Set up our objectPooling
    /// </summary>
    private void Awake()
    {
       pool = new List<GameObject>(poolSize);

       for(int i = 0; i < poolSize; i++ )
        {
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            pool.Add(newObj);
        }
    }

    /// <summary>
    /// Get our gameObjects that are needed, check if there are pool GameObjects,
    /// If not create new one, if canGrow is a true
    /// If all else fails return null
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        if(canGrow)
        {
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            pool.Add(newObj);
            return newObj;
        }

        return null;
    }

    /// <summary>
    /// Collect out gameOjbect and set it active to false, so we can use it later
    /// </summary>
    /// <param name="obj">GameObject we want to set inactive</param>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
