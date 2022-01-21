using Entity;
using UnityEngine;

namespace AI
{
    public class SimplePatrol : PatrolBase
    {
        [Header("Components")]
        [SerializeField] private MovementController movement;

        // Very simple patrol point
        // between two points, dest point must be to the right.
        // patrols from start point to dest point to right.
        
        private Vector3 _startPoint;
        [SerializeField] private Transform destinationPoint;

        private FacingDirection _facingDirection = FacingDirection.Right;

        private const float Speed = 2f;

        private void Start()
        {
            _startPoint = transform.localPosition;
            Debug.Assert(_startPoint.x < destinationPoint.localPosition.x, 
                "This very simple script only allows a patrol destination towards the right of the initial position.", 
                this);
        }

        public override void UpdatePatrol()
        {
            MoveTowardsDestination();
        }

        private void MoveTowardsDestination()
        {
            if (_facingDirection == FacingDirection.Right)
            {
                if (Vector3.Distance(transform.localPosition, destinationPoint.localPosition) < 0.1f)
                {
                    _facingDirection = FacingDirection.Left;
                }
            }
            else
            {
                if (Vector3.Distance(transform.localPosition, _startPoint) < 0.1f)
                {
                    _facingDirection = FacingDirection.Right;
                }
            }
            movement.MoveAtSpeed(new Vector2((float)_facingDirection, 0f) * Speed);
        }
    }
}