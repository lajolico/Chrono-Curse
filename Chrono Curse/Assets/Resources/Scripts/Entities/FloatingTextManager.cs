using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] private ObjectPooling textPool;

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
    }

    private void Start()
    {

    }

    public IEnumerator ShowFloatingText(string message, Vector3 spawnPosition, float displayTime)
    {
        // Instantiate a new instance of the floatingText object
        GameObject textObj = textPool.GetObject();

        // Add a random offset to the spawn position
        float spawnRadius = 1.0f; // adjust this value to control the randomness
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        spawnPosition += new Vector3(randomOffset.x, randomOffset.y, 0.0f);

        textObj.SetActive(true);
        textObj.transform.position = spawnPosition;
        textObj.GetComponent<TextMesh>().text = message;

        yield return new WaitForSeconds(displayTime);

        textPool.ReturnObject(textObj);
    }
}
