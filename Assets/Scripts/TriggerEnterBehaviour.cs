using System;
using DefaultNamespace;
using UnityEngine;
using Object = UnityEngine.Object;

public class TriggerEnterBehaviour : MonoBehaviour
{
    [SerializeField] private bool checkForPlayerOnly;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject levelPrefab;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkForPlayerOnly)
        {
            var level = Instantiate(levelPrefab, offset + transform.parent.position, Quaternion.identity);
            _gameManager.SetCurrentLevelSpawned(level.transform);
        }

        if (other.CompareTag("Level") && !checkForPlayerOnly)
        {
            Destroy(other.gameObject);
        }
    }
}
