using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private readonly List<GameObject> _enemiesToDestroy = new List<GameObject>();
        private List<GameObject> _allEnemySpawnerObjects; // This is a list of enemy objects used as reference for spawning actual enemies. 
        private readonly List<GameObject> _allEnemies = new List<GameObject>();

        private bool _enemiesHaveBeenSpawned = false;

        public static event Action OnEnemiesSpawnedOrDestroyed;

        public static event Action OnAllEnemiesKilled;//AK 4/3/22 Killed is just the enemy having 0 health, not the Game Object destruction

        public void InitEnemies()
        {
            // Get all active spawners, and disable them.
            _allEnemySpawnerObjects = GameObject.FindGameObjectsWithTag("EnemyParent").Where( enemy => enemy.activeSelf ).ToList();
            _allEnemySpawnerObjects.ForEach(e => e.SetActive(false));
        }

        public void SpawnAllEnemies()
        {
            foreach (GameObject enemySpawner in _allEnemySpawnerObjects)
            {
                // Spawn an enemy with this manager as a parent (for tidiness)
                GameObject newEnemy = Instantiate(enemySpawner, transform, true);
                _allEnemies.Add(newEnemy);
                newEnemy.SetActive(true);
            }

            _enemiesHaveBeenSpawned = true;
            OnEnemiesSpawnedOrDestroyed?.Invoke();
        }

        public void DestroyAllEnemies()
        {
            foreach (GameObject enemy in _allEnemies)
            {
                DestroyEnemy(enemy);
            }
            _allEnemies.Clear();
            _enemiesHaveBeenSpawned = false;
            OnEnemiesSpawnedOrDestroyed?.Invoke();
        }

        public void ResetAllEnemies()
        {
            DestroyAllEnemies();
            SpawnAllEnemies();
        }
        
        // Used in the state graph of each enemy.
        [UsedImplicitly] public static void AddForDestruction(GameObject gameObjectToDestroy)
        {
            Instance._enemiesToDestroy.Add(gameObjectToDestroy);
        }

        private bool CheckAllEnemiesDead()
        {
            foreach (GameObject enemy in _allEnemies)
            {
                if (enemy == null) continue;
                enemy.TryGetComponent(out EntityHealth health);
                if (!health) continue;
                if (health.IsAlive())
                {
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            if (!_enemiesHaveBeenSpawned)
            {
                return;
            }
            if (CheckAllEnemiesDead())
            {
                OnAllEnemiesKilled?.Invoke();
            }
            
            Debug.Assert(!_enemiesToDestroy.Contains(gameObject),
                "You're destroying the enemy manager itself, don't do this.", this);
            
            foreach (GameObject gameObjectToDestroy in _enemiesToDestroy)
            {
                // Remove this enemy from the all enemies list
                _allEnemies.Remove(gameObjectToDestroy);
                
                DestroyEnemy(gameObjectToDestroy);
            }
            if (_enemiesToDestroy.Count > 0)
            {
                OnEnemiesSpawnedOrDestroyed?.Invoke();
                _enemiesToDestroy.Clear();
            }
        }

        private void DestroyEnemy(GameObject enemyToDestroy)
        {
            if (!enemyToDestroy)
            {
                Debug.LogError("No enemy to destroy...");
                return;
            }

            // Destroy all children, then parent.
            foreach (Transform child in enemyToDestroy.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(enemyToDestroy);
        }
    }
}