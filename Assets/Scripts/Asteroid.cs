using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 10f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Asteroid : Spawn Manager is NULL");
        }
    }


    // Update is called once per frame
    void Update()
    {
        //rotate object on the Z-axis
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    //check for LASER collission (trigger)
    //instantiate explosion at the position of the asteroid (us)
    //destroy the explosion in 3 sec.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.7f);
        }
    }
}
