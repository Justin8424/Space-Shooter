﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //handle to Text
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartLevelText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _ammoCountText;
    [SerializeField]
    private Image _boostIndicatorImg;
    [SerializeField]
    private Text _waveCountText;

    private GameManager _gameManager;
   
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.Log("Game_Manager is NULL");
        }
    }
    
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if(currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmoCount(int ammo, int maxAmmo)
    {
        _ammoCountText.text = ammo + " /" + maxAmmo;
    }

    public void UpdateBoostIndicator(Color c)
    {
        _boostIndicatorImg.color = c;
    }

    public void UpdateWaveDisplay(int wave, float intermissionDuration)
    {
        _waveCountText.text = "Prepare for Wave: " + wave;
        _waveCountText.gameObject.SetActive(true);
        StartCoroutine(WaveDisplayTime(intermissionDuration));

    }

    IEnumerator WaveDisplayTime(float displayTime)
    {
        yield return new WaitForSeconds(displayTime - 2f);
        _waveCountText.gameObject.SetActive(false);
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartLevelText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    IEnumerator GameOverFlicker()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
