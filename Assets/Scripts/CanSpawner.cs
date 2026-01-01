using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class CanSpawner : MonoBehaviour
    {
        [SerializeField] private Transform leftSpawnerPoint;
        [SerializeField] private Transform middleSpawnerPoint;
        [SerializeField] private Transform rightSpawnerPoint;
        [SerializeField] private List<CanController> canControllers = new List<CanController>();
        [SerializeField] private float spawnTime = 5;
        [SerializeField] private float spawnTimeBetweenEachCan = 1;
        [SerializeField] private int numberOfCansPerSpawn = 5;

        private List<Transform> _spawnPoints = new List<Transform>();
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = FindFirstObjectByType<GameManager>();
            _spawnPoints.Add(leftSpawnerPoint);
            _spawnPoints.Add(middleSpawnerPoint);
            _spawnPoints.Add(rightSpawnerPoint);

            if (canControllers.Count == 0)
            {
                Debug.LogError("No can assigned to can Spawner! Please add at least one obstacle prefab.");
                return;
            }

            StartCoroutine(SpawnCoroutine());
        }

        IEnumerator SpawnCoroutine()
        {
            while (!_gameManager.isGameOver)
            {
                yield return new WaitForSeconds(spawnTime);

                for (int i = 0; i < numberOfCansPerSpawn; i++)
                {
                    yield return new WaitForSeconds(spawnTimeBetweenEachCan);
                    int randomSpawnIndex = Random.Range(0, _spawnPoints.Count);
                    int randomObstacleIndex = Random.Range(0, canControllers.Count);
                    var firstObstacle = Instantiate(
                        canControllers[randomObstacleIndex],
                        _spawnPoints[randomSpawnIndex].position,
                        Quaternion.identity
                    );

                    int nextSpawnIndex = randomSpawnIndex + 1;
                    if (nextSpawnIndex >= _spawnPoints.Count)
                    {
                        nextSpawnIndex = 0;
                    }

                    var secondObstacle = Instantiate(
                        canControllers[randomObstacleIndex],
                        _spawnPoints[nextSpawnIndex].position,
                        Quaternion.identity
                    );
                    firstObstacle.transform.parent = _gameManager.GetCurrentLevelSpawned();
                    secondObstacle.transform.parent = _gameManager.GetCurrentLevelSpawned();
                }
            }
        }
    }
}