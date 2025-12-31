using System;
using System.Globalization;
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
        public bool isGameOver { get; set; }
        private float _currentScore;
        private Transform _currentLevel;

        private void Start()
        {
            _currentScore = 0;
            scoreText.text = _currentScore.ToString(CultureInfo.InvariantCulture);
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
           var levels =FindObjectsByType<LevelMovement>(FindObjectsSortMode.None);
           foreach (var level in levels)
           {
               level.isGameOver = isGameOver;
           }
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