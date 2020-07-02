using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerupToSpawn
{
    public GameObject powerup;
    public float spawnRate;
    [HideInInspector] public float minSpawnProb, maxSpawnProb;
}

[System.Serializable]
public class EnemyToSpawn
{
    public GameObject enemy;
    public float spawnRate;
    [HideInInspector] public float minSpawnProb, maxSpawnProb;
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private int _enemiesThisWave = 6;
    private int _enemiesSpawned;
    [SerializeField]
    private int _enemiesToAdd = 2;
    [SerializeField]
    private float _waveTimer = 7f;
    private int _waveCount = 1;

    private bool _stopSpawning = false;

    UIManager _uIManager;

    public PowerupToSpawn[] powerupToSpawn;
    public EnemyToSpawn[] enemyToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        for (int i = 0; i < powerupToSpawn.Length; i++)
        {
            if (i == 0)
            {
                powerupToSpawn[i].minSpawnProb = 0;
                powerupToSpawn[i].maxSpawnProb = powerupToSpawn[i].spawnRate - 1;
            }
            else
            {
                powerupToSpawn[i].minSpawnProb = powerupToSpawn[i - 1].maxSpawnProb + 1;
                powerupToSpawn[i].maxSpawnProb = powerupToSpawn[i].minSpawnProb + powerupToSpawn[i].spawnRate - 1;
            }

        }

        for (int i = 0; i < enemyToSpawn.Length; i++)
        {
            if (i == 0)
            {
                enemyToSpawn[i].minSpawnProb = 0;
                enemyToSpawn[i].maxSpawnProb = enemyToSpawn[i].spawnRate - 1;
            }
            else
            {
                enemyToSpawn[i].minSpawnProb = enemyToSpawn[i - 1].maxSpawnProb + 1;
                enemyToSpawn[i].maxSpawnProb = enemyToSpawn[i].minSpawnProb + enemyToSpawn[i].spawnRate - 1;
            }

        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2f);
        while (_stopSpawning == false)
        {
            int randomEnemy = Random.Range(0, 100);
            for (int i = 0; i< enemyToSpawn.Length; i++)
            {
                if (randomEnemy >= enemyToSpawn[i].minSpawnProb && randomEnemy <= enemyToSpawn[i].maxSpawnProb)
                {
                    GameObject newEnemy = Instantiate(enemyToSpawn[i].enemy, new Vector3(Random.Range(-8f, 8f), 7f, 0), Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                    _enemiesSpawned++;
                    break;
                }
            }
            if (_enemiesSpawned == _enemiesThisWave)
            {
                _waveCount++;
                _enemiesThisWave += _enemiesToAdd;
                _uIManager.UpdateWaveDisplay(_waveCount, _waveTimer);
                yield return new WaitForSeconds(_waveTimer);
                _enemiesSpawned = 0;
            }
            yield return new WaitForSeconds(5.0f);
        }
    }
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(2f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7f, 0);
            int randomPowerUp = Random.Range(0, 100);
            for (int i = 0; i < powerupToSpawn.Length; i++)
            {
                if (randomPowerUp >= powerupToSpawn[i].minSpawnProb && randomPowerUp <= powerupToSpawn[i].maxSpawnProb)
                {
                    Instantiate(powerupToSpawn[i].powerup, posToSpawn, Quaternion.identity);
                    break;
                }
            }
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
        
}
