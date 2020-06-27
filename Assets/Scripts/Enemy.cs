using System.Collections;
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
    private float _bombDropRate = 1.5f;
    [SerializeField]
    private float _canFire = -1; 
    [SerializeField]
    private GameObject _enemyLaserPrefab;
    [SerializeField]
    private float _chaseRange = 5;
    [SerializeField]
    private int _shieldhealth = 1;
    private bool _shieldsActive = false;
    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private float _slolamRange = 3;

    [SerializeField]
    private GameObject _minePrefab;

    Vector3 _posOffSet = new Vector3();

    private void Start()
    {
        _posOffSet = transform.position;
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

        if (this.gameObject.name == "Shield_Enemy" || this.gameObject.name == "Shield_Enemy(clone)")
        {
            _shieldsActive = true;
        }       
    }

    void Update()
    {
        CalculateMovement();
        ShootPickup();
        ChasePlayer();
        if(Time.time > _canFire && gameObject.tag != "Mine Layer" )
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
        if (Time.time > _canFire && gameObject.tag == "Mine Layer")
        {
            _bombDropRate = Random.Range(1f, 5f);
            _canFire = Time.time + _bombDropRate;
            GameObject enemyMine = Instantiate(_minePrefab, transform.position, Quaternion.identity);
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

        if (gameObject.tag == "Mine Layer")
        {
           Vector3 tempPos = new Vector3();
            /*tempPos = _posOffSet;
            tempPos.x += Mathf.Cos(Time.time * _speed) * _slolamRange;
            transform.position = tempPos;*/
            transform.Translate(new Vector3(Mathf.Cos(Time.time * (_speed)) * _slolamRange, -.1f, 0));
        }
        else
        {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (transform.position.y < -5.5f)
        {
            Destroy(this.gameObject);
        }
    }

    void ShootPickup()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * 6f, Color.green);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), 6f);

        if (hit.collider !=null && hit.collider.tag == "Pickup")
        {
            Debug.Log("Shooting Pickup");
            _fireRate = -1;
        }
        else
        {
            ;
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
            if (_shieldsActive == false)
            {
                if (_player != null)
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
            } else if (_shieldsActive == true)
            {
                Destroy(other.gameObject);
                Damage();
            }

        }
    }

    void Damage()
    {
        _shieldhealth--;
        if (_shieldhealth == 0)
        {
            _shield.SetActive(false);
            _shieldsActive = false;
        }
    }
}
