using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private readonly List<GameObject> _enemiesToDestroy = new List<GameObject>();

        public delegate void AllEnemiesKilled(); //AK 4/3/22 Killed is just the enemy having 0 health, not the Game Object destruction
        public static event AllEnemiesKilled OnAllEnemiesKilled;

        [UsedImplicitly] public static void AddForDestruction(GameObject gameObjectToDestroy)
        {
            Instance._enemiesToDestroy.Add(gameObjectToDestroy);
        }

        private bool CheckAllEnemiesDead()
        {
            bool enemiesDead;
            GameObject parent = gameObject.transform.parent.gameObject;
            int enemyCount = parent.transform.childCount;
            if (enemyCount <= 1) //less than 1 to account for the enemy manager itself
            {
                enemiesDead = true;
            }
            else
            {
                enemiesDead = false;
            }
            return enemiesDead;
        }

        private void Update()
        {
            if (_enemiesToDestroy.Count == 0)
            {
                if (CheckAllEnemiesDead()) OnAllEnemiesKilled();
                return;
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