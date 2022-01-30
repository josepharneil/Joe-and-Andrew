using System;
using UnityEngine;

namespace Entity
{
    public class EntityController : MonoBehaviour
    {
        public event Action OnEnemyDead;

        public GameObject enemyParentGameObject;

        public void InvokeEnemyDeadEvent()
        {
            OnEnemyDead?.Invoke();
        }
    }
}