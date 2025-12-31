using AudioManagement;
using DefaultNamespace;
using UnityEngine;

public class Obstacle : MonoBehaviour
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
            _gameManager.HandleGameOver();
            other.GetComponent<PlayerMovement>().HandleGameOver();
            AudioManager.Instance.Play(Sound.Can.ToString());
        }
    }
}
