using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Entity
{
    public class EntityController : MonoBehaviour
    {
        public event Action OnEnemyDead;

        [UsedImplicitly] public GameObject enemyParentGameObject;
        
        [UsedImplicitly] public void InvokeEnemyDeadEvent()
        {
            OnEnemyDead?.Invoke();
        }
    }
}