using Enemy;
using JetBrains.Annotations;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(Gateway))]
    public class GatewayLock : MonoBehaviour
    {
        [SerializeField] private Animator gateAnimator;
        [SerializeField] private Gateway gateway;
        private static readonly int IsLocked = Animator.StringToHash("_isLocked");

        private void OnEnable()
        {
            EnemyManager.OnAllEnemiesKilled += StartUnlockGate;
        }

        private void OnDisable()
        {
            EnemyManager.OnAllEnemiesKilled -= StartUnlockGate;
        }

        private void StartUnlockGate()
        {
            gateAnimator.SetBool(IsLocked, false);
        }

        // This is called by the animator so it happens at the right time
        [UsedImplicitly] public void Unlock()
        {
            gateway.IsLocked = false;
        }
    }
}