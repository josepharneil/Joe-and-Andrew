using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class SimplePatrol : PatrolBase
    {
        [Header("Components")]
        [SerializeField] private EnemyMovement movement;

        // Very simple patrol point
        // between two points, dest point must be to the right.
        // patrols from start point to dest point to right.
        
        private Vector3 _startPoint;
        [SerializeField] private Transform destinationPoint;

        private FacingDirection _facingDirection = FacingDirection.Right;

        private const float Speed = 2f;

        private void Start()
        {
            _startPoint = transform.position;
        }

        public override void UpdatePatrol()
        {
            MoveTowardsDestination();
        }

        private void MoveTowardsDestination()
        {
            if (_facingDirection == FacingDirection.Right)
            {
                if (Vector3.Distance(transform.position, destinationPoint.position) < 0.1f)
                {
                    _facingDirection = FacingDirection.Left;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, _startPoint) < 0.1f)
                {
                    _facingDirection = FacingDirection.Right;
                }
            }
            movement.MoveAtSpeed(new Vector2(Speed * (float)_facingDirection, 0f));
        }
    }
}