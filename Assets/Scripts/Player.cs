using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.5f;
    private float _baseSpeed;
    private bool _isBoosting;
    [SerializeField]
    private float _boostSpeed = 5.25f;
    [SerializeField]
    private float _speedMultiplier = 3f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
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

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audiosource = GetComponent<AudioSource>();
        _baseSpeed = _speed;

        if(_spawnManager == null)
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
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isBoosting = true;
        }
        else
        {
            _isBoosting = false;
        }
        if(_isBoosting == true)
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
                Instantiate(_tripleShotPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                _ammoCount--;
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
                    _shieldHealth--;
                    _isSheildsActive = false;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.white;
                    _shieldVisualizer.SetActive(false);
                    break;
                case 2:
                    _shieldHealth--;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                case 3:
                    _shieldHealth--;
                    _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;

            }
        } 
        else if (_isSheildsActive == false)
        {
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

    public void SpeedupActive()
    {
        _isSpeedupActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedupPowerDown());
    }

    IEnumerator SpeedupPowerDown()
    {
        if (_isSpeedupActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isSpeedupActive = false;
            _speed /= _speedMultiplier;
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
        _uiManager.UpdateAmmoCount(_ammoCount);
    }

}