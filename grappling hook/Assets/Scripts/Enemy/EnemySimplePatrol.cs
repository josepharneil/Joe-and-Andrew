using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySimplePatrol : MonoBehaviour
    {
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private Transform patrolPositionRight;
        [SerializeField] private Transform patrolPositionLeft;
        [SerializeField] private Transform player;
        
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private FacingDirection _facingDirection = FacingDirection.Right;
        
        /// <summary>
        /// Called by Bolt.
        /// </summary>
        /// <param name="speed"></param>
        public void PatrolBetweenPositions(float speed)
        {
            if (_facingDirection == FacingDirection.Right)
            {
                movement.MoveAtSpeed(Vector2.right * speed);
            }
            else
            {
                movement.MoveAtSpeed(Vector2.left * speed);
            }

            const float threshold = 0.1f;
            if (Math.Abs(_transform.position.x - patrolPositionRight.position.x) < threshold)
            {
                _facingDirection = FacingDirection.Left;
            }
            else if ((Math.Abs(_transform.position.x - patrolPositionLeft.position.x) < threshold))
            {
                _facingDirection = FacingDirection.Right;
            }
        }

        public bool CanSeePlayer(float distance)
        {
            return Vector2.Distance(_transform.position, player.position) < distance;

            // ContactFilter2D contactFilter2D = new ContactFilter2D
            // {
            //     layerMask = LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Ground"),
            //     useLayerMask = true,
            // };
            // List<RaycastHit2D> detectedObjects = new List<RaycastHit2D>();
            // var position = _transform.position;
            // var playerPosition = player.position;
            // Physics2D.Raycast(position,  
            //     playerPosition - position, 
            //     contactFilter2D, detectedObjects, distance);
            //
            // Debug.DrawRay(position, playerPosition - position);
            //
            // if (detectedObjects.Count < 2)
            // {
            //     return false;
            // }
            //
            // print(detectedObjects[1].transform.name);
            //
            // return detectedObjects[1].transform.gameObject.layer == LayerMask.NameToLayer("Player");
        }
    }
}
