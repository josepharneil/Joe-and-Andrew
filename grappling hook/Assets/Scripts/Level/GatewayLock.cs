using Enemy;
using Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace level
{
    [RequireComponent(typeof(Gateway))]
    public class GatewayLock : MonoBehaviour
    {
        [SerializeField] private Animator gateAnimator;
        [SerializeField] private Gateway gateway;
        private void OnEnable()
        {
            EnemyManager.OnAllEnemiesKilled += StartUnlockGate;
        }

        private void OnDisable()
        {
            EnemyManager.OnAllEnemiesKilled -= StartUnlockGate;
        }

        public void StartUnlockGate()
        {
 
            gateAnimator.SetBool("_isLocked", false);
        }

        //This is called by the animator so it happens at the right time
        public void SetGetUnlocked()
        {
            gateway._isLocked = false;
        }
    }
}