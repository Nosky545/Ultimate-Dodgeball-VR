using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public GameObject gameScreen;

    public GameObject enemyPrefab;

    public TextMeshProUGUI enemyLeftText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI lifeText;

    public bool isGameActive;

    public float enemyCount;
    public int waveCount = 1;
    public int lifeCount = 3;

    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemyLeftText.text = "Enemy Left: " + enemyCount;
        roundText.text = "Round: " + waveCount;
        lifeText.text = "Lives: " + lifeCount;

        if (enemyCount == 0 && isGameActive)
        {
            waveCount++;
            SpawnEnemyWave(waveCount);
        }

        if (lifeCount == 0)
        {
            GameOver();
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-8.5f, 8);
        Vector3 randomPos = new Vector3(spawnPosX, 1, 12.5f);
        return randomPos;
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        isGameActive = false;
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        titleScreen.SetActive(false);
        gameScreen .SetActive(true);
        SpawnEnemyWave(waveCount);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
