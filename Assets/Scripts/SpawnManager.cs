using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject []_powerups;
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

    // Start is called before the first frame update
    void Start()
    {
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
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
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-8f, 8f), 7f, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            _enemiesSpawned++;
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
            int randomPowerUp = Random.Range(0, _powerups.Length);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
        
}
