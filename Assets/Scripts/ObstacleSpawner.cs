using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private Transform leftSpawnerPoint;
    [SerializeField] private Transform middleSpawnerPoint;
    [SerializeField] private Transform rightSpawnerPoint;
    [SerializeField] private List<Obstacle> obstacles = new List<Obstacle>();
    [SerializeField] private float spawnTime = 3;
    private List<Transform> _spawnPoints = new List<Transform>();
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _spawnPoints.Add(leftSpawnerPoint);
        _spawnPoints.Add(middleSpawnerPoint);
        _spawnPoints.Add(rightSpawnerPoint);
        
        if (obstacles.Count == 0)
        {
            Debug.LogError("No obstacles assigned to ObstacleSpawner! Please add at least one obstacle prefab.");
            return;
        }
        
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (!_gameManager.isGameOver)
        {
            yield return new WaitForSeconds(spawnTime);
            
            int randomSpawnIndex = Random.Range(0, _spawnPoints.Count);
            int randomObstacleIndex = Random.Range(0, obstacles.Count);
            var firstObstacle = Instantiate(
                obstacles[randomObstacleIndex], 
                _spawnPoints[randomSpawnIndex].position,
                Quaternion.identity
            );
            
            int nextSpawnIndex = randomSpawnIndex + 1;
            if (nextSpawnIndex >= _spawnPoints.Count)
            {
                nextSpawnIndex = 0;
            }
            var secondObstacle = Instantiate(
                obstacles[randomObstacleIndex], 
                _spawnPoints[nextSpawnIndex].position,
                Quaternion.identity
            );
            firstObstacle.transform.parent = _gameManager.GetCurrentLevelSpawned();
            secondObstacle.transform.parent = _gameManager.GetCurrentLevelSpawned();
        }
    }
}