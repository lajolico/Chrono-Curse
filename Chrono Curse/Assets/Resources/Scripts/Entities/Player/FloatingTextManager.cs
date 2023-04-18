using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] private ObjectPooling textPool;

    private float displayTime = 0.9f;

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

    public IEnumerator ShowFloatingText(string message)
    {
        // Instantiate a new instance of the floatingText object
        GameObject textObj = textPool.GetObject();

        // Set the spawn position to the player's position
        Vector3 spawnPosition = PlayerManager.Instance.GetPlayerPosition();

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
