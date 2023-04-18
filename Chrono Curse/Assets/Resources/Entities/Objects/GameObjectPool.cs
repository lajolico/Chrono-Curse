using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool 
{
    private GameObject prefab;
    private Transform parentTransform;
    private List<GameObject> pool;

    public GameObjectPool(GameObject prefab, Transform parentTransform)
    {
        this.prefab = prefab;
        this.parentTransform = parentTransform;
        this.pool = new List<GameObject>();
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.gameObject.activeSelf)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Object.Instantiate(prefab, parentTransform);
        pool.Add(newObj);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }
}
