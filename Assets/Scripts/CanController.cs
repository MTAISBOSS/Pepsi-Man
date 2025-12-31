using System;
using AudioManagement;
using DefaultNamespace;
using UnityEngine;

public class CanController : MonoBehaviour
{
    private GameManager _gameManager;
    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _gameManager.IncreaseScore();
            AudioManager.Instance.Play(Sound.Can.ToString());
            Destroy(gameObject);
        }
    }
}
