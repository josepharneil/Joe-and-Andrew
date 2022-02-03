using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private readonly List<GameObject> _enemiesToDestroy = new List<GameObject>();

        [UsedImplicitly] public void AddForDestruction(GameObject gameObjectToDestroy)
        {
            _enemiesToDestroy.Add(gameObjectToDestroy);
        }

        private void Update()
        {
            if (_enemiesToDestroy.Count == 0) return;
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