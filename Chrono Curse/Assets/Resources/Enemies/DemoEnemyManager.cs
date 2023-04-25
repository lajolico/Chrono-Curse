using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class DemoEnemyManager : MonoBehaviour
{
    public static DemoEnemyManager Instance { get; private set; }

    [SerializeField] private List<GameObject> activeEnemies = new List<GameObject>();
    [SerializeField] internal List<EnemyObject> enemyDataList = new List<EnemyObject>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private int maxActiveEnemies = 5;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private GameObject enemyPrefab;
    public TextMeshProUGUI countdownText;
    public float countdownTime = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(CountdownAndSpawnEnemies());
    }

    private IEnumerator CountdownAndSpawnEnemies()
    {
        // Display "Survive!" message
        countdownText.text = "Survive!";

        // Wait for a moment before starting the countdown
        yield return new WaitForSeconds(1f);

        // Start the countdown
        float timeLeft = countdownTime;
        while (timeLeft > 0)
        {
            countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        countdownText.gameObject.SetActive(false);

        List<EnemyObject> possibleEnemies = enemyDataList;

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Spawn enemies until we reach our max active limit
            while (activeEnemies.Count < maxActiveEnemies)
            {
                // Choose a random spawn point
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

                // Spawn the enemy
                // Spawn enemy prefab
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.SetAttackDamage(PlayerManager.Instance.currentLevel);
                enemyScript.SetPropertiesFromObjectData(possibleEnemies[Random.Range(0, possibleEnemies.Count)],
                    enemy.GetComponent<Animator>(), enemy.GetComponentInChildren<AttackChecker>().gameObject);
                activeEnemies.Add(enemy);

            }
        }
    }
    internal void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void Reset()
    {
        foreach (var enemy in activeEnemies)
        {
            Destroy(enemy);
        }
    }
}