using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class TruckSpawner : MonoBehaviour
{
    [SerializeField] private Transform leftSpawnerPoint;
    [SerializeField] private Transform rightSpawnerPoint;
    [SerializeField] private float spawnTime = 5;
    private List<Transform> _spawnPoints = new List<Transform>();
    private GameManager _gameManager;
    [SerializeField] private Transform truckPrefab;
    [SerializeField] private float destroyTime;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _spawnPoints.Add(leftSpawnerPoint);
        _spawnPoints.Add(rightSpawnerPoint);

        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (!_gameManager.isGameOver)
        {
            yield return new WaitForSeconds(spawnTime);

                int randomSpawnIndex = Random.Range(0, _spawnPoints.Count);
                var firstObstacle = Instantiate(
                    truckPrefab,
                    _spawnPoints[randomSpawnIndex].position,
                    Quaternion.identity
                );
                Destroy(firstObstacle.gameObject,destroyTime);
        }
    }
}