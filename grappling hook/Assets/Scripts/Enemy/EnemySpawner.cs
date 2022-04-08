using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyToSpawn;
        [SerializeField] private GameObject _spawnPosition;

        [SerializeField] private bool _spawnerEnabled = false;
        [SerializeField] private int _maxNumberOfSpawnsAtOnce = 5;
        
        [SerializeField] private float _spawnEveryXSeconds = 10f;
        private float _spawnTimer = 0f;

        private List<GameObject> _allSpawnedEnemies = new List<GameObject>();

        private void SpawnEnemy()
        {
            // Might want to track 
            GameObject enemy = Instantiate(_enemyToSpawn, _spawnPosition.transform.position, Quaternion.identity, transform);
            enemy.SetActive(true);
            _allSpawnedEnemies.Add(enemy);
        }

        private void Update()
        {
            if (!_spawnerEnabled) return;

            List<int> indexesToDelete = new List<int>();
            for (int i = 0; i < _allSpawnedEnemies.Count; i++)
            {
                if (_allSpawnedEnemies[i] == null)
                {
                    indexesToDelete.Add(i);
                }
            }
            foreach (var deleteIndex in indexesToDelete)
            {
                _allSpawnedEnemies.RemoveAt(deleteIndex);
            }

            if (_allSpawnedEnemies.Count > _maxNumberOfSpawnsAtOnce)
            {
                return;
            }

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < _spawnEveryXSeconds) return;

            _spawnTimer = 0f;
            SpawnEnemy();
        }
    }
}