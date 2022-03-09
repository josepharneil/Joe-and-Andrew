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
        private List<GameObject> _allEnemies;

        public static event Action OnAllEnemiesKilled;//AK 4/3/22 Killed is just the enemy having 0 health, not the Game Object destruction

        private void Start()
        {
            // Get all active enemy tagged objects.
            _allEnemies = GameObject.FindGameObjectsWithTag("Enemy").Where( enemy => enemy.activeSelf ).ToList();
        }

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
            if (CheckAllEnemiesDead())
            {
                OnAllEnemiesKilled?.Invoke();
            }
            
            Debug.Assert(!_enemiesToDestroy.Contains(gameObject),
                "You're destroying the enemy manager itself, don't do this.", this);
            foreach (GameObject gameObjectToDestroy in _enemiesToDestroy)
            {
                foreach (Transform child in gameObjectToDestroy.transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(gameObjectToDestroy);
            }
            _enemiesToDestroy.Clear();
        }
    }
}