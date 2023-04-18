using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    public GameObject floatingTextPrefab;
    public int poolSize = 10;
    public Transform textParent;
    private GameObjectPool pool;

    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pool = new GameObjectPool(floatingTextPrefab, textParent);

        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = pool.GetObject();
            obj.SetActive(false);
        }
    }

    public void ShowFloatingText(string message, int count)
    {
        // Instantiate a new instance of the floatingText object
        GameObject floatingText = pool.GetObject();

        floatingText.transform.position = transform.position;
        floatingText.GetComponentInChildren<TextMesh>().text = message + " x " + count;
        floatingText.gameObject.SetActive(true);

        pool.ReturnObject(floatingText);
    }
}
