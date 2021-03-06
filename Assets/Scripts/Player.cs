﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed;
    private float _baseSpeed = 3.5f;
    private bool _isBoosting;
    [SerializeField]
    private float _boostSpeed = 5.25f;
    private float _currentBoostDelayTime;
    private float _currentBoostTime;
    [SerializeField]
    private float _boostTime = 3f;
    [SerializeField]
    private float _boostDelayTime = 5f;
    [SerializeField]
    private float _speedMultiplier = 3f;
    [SerializeField]
    private float _speedDownMultiplier = .4f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _threeWayShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    private int _ammoCount = 15;
    [SerializeField]
    private int _maximumAmmo = 15;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedupActive = false;
    private bool _isSheildsActive = false;
    private bool _isThreeWayShotActive = false;
    private bool _isEngineSludgeActive = false;

    [SerializeField]
    private int _maxThreeWayShots = 5;
    private int _threeWayShotsLeft;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;

    //variable to store the audio clip
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _noAmmo;
    [SerializeField]
    private AudioSource _audiosource;

    [SerializeField]
    private int _shieldStrength = 3;
    [SerializeField]
    private int _shieldHealth;

    CameraShake _cameraShake;
    [SerializeField]
    private float _shakeDuration = .10f;
    [SerializeField]
    private float _shakeMagnitude = .35f;

    private GameObject[] _pickups;
    [SerializeField]
    private float _pullSpeed = 2;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audiosource = GetComponent<AudioSource>();
         _speed = _baseSpeed;
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _currentBoostTime = 0f;
        _currentBoostDelayTime = 0f;

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
       
        if(_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if(_audiosource == null)
        {
            Debug.LogError("The Audio Source on the player is NULL.");
        }
        else
        {
            _audiosource.clip = _laserSoundClip;
        }

    }

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
        Ammo();
        PullPickups();
    }

    void PullPickups()
    {
        if (Input.GetKey(KeyCode.C))
        {
            _pickups = GameObject.FindGameObjectsWithTag("Pickup");
            foreach(var pickup in _pickups)
            {
                pickup.transform.position = Vector3.MoveTowards(pickup.transform.position, this.transform.position, (_pullSpeed * Time.deltaTime));
            }
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && !_isBoosting && Time.time > _currentBoostDelayTime)
        {
            _uiManager.UpdateBoostIndicator(Color.green);
            _currentBoostTime = Time.time + _boostTime;
            _isBoosting = true;
        }
        
        if ((Time.time > _currentBoostTime) && _isBoosting)
        {
            _uiManager.UpdateBoostIndicator(Color.red);
            _isBoosting = false;
            _currentBoostDelayTime = Time.time + _boostDelayTime;
        }

        if (Time.time > _currentBoostDelayTime)
        {
            _uiManager.UpdateBoostIndicator(Color.green);
        }

        if (_isBoosting == true)
        {
            _speed = _boostSpeed;
        }
        else if (_isBoosting == false)
        {
            _speed = _baseSpeed;
        }

        transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x >= 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x <= -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }


    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_ammoCount > 0)
        {
            if (_isTripleShotActive == true)
            {
                _isThreeWayShotActive = false;
                _threeWayShotsLeft = 0;
                Instantiate(_tripleShotPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                _ammoCount--;
            }
            else if(_isThreeWayShotActive == true)
            {
                if (_threeWayShotsLeft > 0)
                {
                    _isTripleShotActive = false;
                    _threeWayShotsLeft--;
                    Instantiate(_threeWayShotPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    _ammoCount--;
                } 
                else if(_threeWayShotsLeft <= 0)
                {
                    _isThreeWayShotActive = false;
                    return;
                }
            }
            else
            {
                Instantiate(_laserPrefab, new Vector3(transform.position.x, transform.position.y + 1.05f, 0), Quaternion.identity);
                _ammoCount--;
            }
            //may have to reassign laser sound effect after adding ammo refill powerup
            _audiosource.Play();
        }
        else if (_ammoCount <= 0)
        {
            _ammoCount = 0;
            _audiosource.clip = _noAmmo;
            _audiosource.Play();
        }

    }

    public void Damage()
    {
        if (_isSheildsActive == true)
        {
            switch (_shieldHealth)
            {
                case 1:
                    StartCoroutine(_cameraShake.Shake(_shakeDuration, (_shakeMagnitude / 2)));
                    _shieldHealth--;
                    _isSheildsActive = false;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.white;
                    _shieldVisualizer.SetActive(false);
                    break;
                case 2:
                    StartCoroutine(_cameraShake.Shake(_shakeDuration, (_shakeMagnitude / 2)));
                    _shieldHealth--;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                case 3:
                    StartCoroutine(_cameraShake.Shake(_shakeDuration, (_shakeMagnitude / 2)));
                    _shieldHealth--;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;

            }
        } 
        else if (_isSheildsActive == false)
        {
            StartCoroutine(_cameraShake.Shake(_shakeDuration, _shakeMagnitude));
            _lives--;
        }

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void ThreeWayShotActive()
    {
        _isThreeWayShotActive = true;
        _threeWayShotsLeft = _maxThreeWayShots;
    }

    public void SpeedupActive()
    {
        _isSpeedupActive = true;
        _baseSpeed *= _speedMultiplier;
        StartCoroutine(SpeedupPowerDown());
    }

    IEnumerator SpeedupPowerDown()
    {
        if (_isSpeedupActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isSpeedupActive = false;
            _baseSpeed /= _speedMultiplier;
        }
    }

    public void EngineSludgeActive()
    {
        _isEngineSludgeActive = true;
        _baseSpeed *= _speedDownMultiplier;
        _boostSpeed *= _speedDownMultiplier;
        StartCoroutine(EngineSludgePowerDown());
    }

    IEnumerator EngineSludgePowerDown()
    {
        if (_isEngineSludgeActive == true)
        {
            yield return new WaitForSeconds(4.0f);
            _isEngineSludgeActive = false;
            _baseSpeed /= _speedDownMultiplier;
            _boostSpeed /= _speedDownMultiplier;
        }
    }

    public void AmmoPowerUp()
    {
        _ammoCount = _maximumAmmo;
        _audiosource.clip = _laserSoundClip;
    }

    public void ExtraLifePickup()
    {
        _lives++;
        if (_lives >= 3)
        {
            _lives = 3;
            _rightEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _leftEngine.SetActive(false);
        }
        _uiManager.UpdateLives(_lives);
    }

    public void ShieldsActive()
    {
        _isSheildsActive = true;
        _shieldHealth = _shieldStrength;
        _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.white;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void Ammo()
    {
        _uiManager.UpdateAmmoCount(_ammoCount, _maximumAmmo);
    }

}