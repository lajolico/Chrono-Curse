using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    private void LoadEnemies(EnemyData enemyData)
    {
        // Spawn enemies from the enemy data
        foreach (EnemyState enemyState in enemyData.enemies)
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Enemies/Enemy"); //CHANGE THIS
            GameObject enemyObject = Instantiate(enemyPrefab, enemyState.position, enemyState.rotation);
            enemyObject.transform.localScale = enemyState.scale;
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            enemy.currentHealth = enemyState.health;
            AddEnemy(enemyObject);
        }
    }
}
