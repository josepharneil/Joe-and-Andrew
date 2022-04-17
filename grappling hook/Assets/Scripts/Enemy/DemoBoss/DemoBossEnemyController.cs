using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

namespace Enemy
{
    public class DemoBossEnemyController : MonoBehaviour
    {
        [Header("Components")]
        [HideInInspector] public MovementController MovementController;
        [SerializeField] private BoxCollider2D arenaBox;
        [HideInInspector] public GameObject target;

        // [] charging
        private Vector3 _chargeTargetPosition;
        [SerializeField] private float _chargeSpeed = 5f;
        [SerializeField] private float _chargeDuration = 2f;

        public bool TargetIsInBossArea()
        {
            return arenaBox.bounds.Contains(target.transform.position);
        }
    }
}