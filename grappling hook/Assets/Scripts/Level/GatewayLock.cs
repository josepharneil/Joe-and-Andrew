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
        [SerializeField] private SpriteRenderer gateRenderer;
        private Animator gateAnimator;
        [SerializeField] private Gateway gateway;
        private void OnEnable()
        {
            EnemyManager.OnAllEnemiesKilled += UnlockGate;
            gateRenderer.color = Color.red;
        }

        private void OnDisable()
        {
            EnemyManager.OnAllEnemiesKilled -= UnlockGate;
        }

        public void UnlockGate()
        {
            gateRenderer.color = Color.green;
            gateway._isLocked = false;
            //gateAnimator.SetBool("_isLocked", false);
        }
    }
}