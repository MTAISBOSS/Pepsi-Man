﻿using System;
using System.Globalization;
using System.Collections; // Add for coroutines
using AudioManagement;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float baseScoreFactor;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject gameOverCamera;
        [SerializeField] private LevelParabolaManager parabolaManager;

        [SerializeField] private float minThresholdChangeTime;
        [SerializeField] private float maxThresholdChangeTime;
        [SerializeField] private float maxThresholdMagnitude;
        
        public bool isGameOver { get; private set; }
        private float _currentScore;
        private Transform _currentLevel;
        private Coroutine _thresholdCoroutine;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI resultScreenScoreText;
        [SerializeField] private TextMeshProUGUI resultScreenHighestScoreText;

        private void Start()
        {
            _currentScore = 0;
            scoreText.text = _currentScore.ToString(CultureInfo.InvariantCulture);
            StartThresholdControl();
        }

        private void StartThresholdControl()
        {
            if (_thresholdCoroutine != null)
            {
                StopCoroutine(_thresholdCoroutine);
            }
            
            if (!isGameOver)
            {
                _thresholdCoroutine = StartCoroutine(ControlXThreshold());
            }
        }

        private IEnumerator ControlXThreshold()
        {
            while (!isGameOver)
            {
                float waitTime = UnityEngine.Random.Range(minThresholdChangeTime, maxThresholdChangeTime);
                yield return new WaitForSeconds(waitTime);
                if (!isGameOver && parabolaManager != null)
                {
                    float randomTarget = UnityEngine.Random.Range(-maxThresholdMagnitude, maxThresholdMagnitude);
                    parabolaManager.SetTargetXThreshold(randomTarget);
                }
            }
        }
        public void IncreaseScore()
        {
            _currentScore += baseScoreFactor;
            scoreText.text = _currentScore.ToString(CultureInfo.InvariantCulture);
        }

        public void HandleGameOver()
        {
            isGameOver = true;
            gameOverCamera.SetActive(true);
            AudioManager.Instance.Play(Sound.Death.ToString());
            var highScore = PlayerPrefs.GetInt("Score", 0);
            if (highScore < _currentScore)
            {
                PlayerPrefs.SetFloat("Score", _currentScore);
            }
            if (_thresholdCoroutine != null)
            {
                StopCoroutine(_thresholdCoroutine);
                _thresholdCoroutine = null;
            }
            
            var levels = FindObjectsByType<DirectionMovement>(FindObjectsSortMode.None);
            foreach (var level in levels)
            {
                level.isGameOver = isGameOver;
            }

            gameOverPanel.SetActive(true);
            resultScreenScoreText.text = _currentScore.ToString(CultureInfo.InvariantCulture);
            resultScreenHighestScoreText.text = PlayerPrefs.GetFloat("Score").ToString(CultureInfo.InvariantCulture);
        }

        public Transform GetCurrentLevelSpawned()
        {
            return _currentLevel;
        }

        public void SetCurrentLevelSpawned(Transform level)
        {
            _currentLevel = level;
        }
    }
}