﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;

    //handle to animator component
    private Animator _anim;

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;
    [SerializeField]
    private float _fireRate = 3.0f;
    [SerializeField]
    private float _canFire = -1; 
    [SerializeField]
    private GameObject _enemyLaserPrefab;
    [SerializeField]
    private float _chaseRange = 5;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }
        _anim = GetComponent<Animator>();

        if(_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Enemy Audio Source is NULL");
        }
        else
        {
            _audioSource.clip = _explosionSound;
        }
        if (_enemyLaserPrefab == null)
        {
            Debug.LogError("The Enemy Laser Prefab is NULL");
        }
        
    }

    void Update()
    {
        CalculateMovement();
        ChasePlayer();
        if(Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i<lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void ChasePlayer()
    {
        if (this.gameObject.name == "Kamikaze(Clone)")
        {
            float dist = Vector3.Distance(_player.transform.position, transform.position);
            if (dist >= _chaseRange)
            {
                transform.position = Vector2.MoveTowards
                    (
                    transform.position, new Vector2(_player.transform.position.x, transform.position.y), _speed * Time.deltaTime
                    );
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            //trigger anim

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            if(_player != null)
            {
                _player.AddScore(10);
            }
            Destroy(other.gameObject);
            //trigger anim
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }
}
