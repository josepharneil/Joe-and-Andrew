using System;
using Entity;
using Physics;
using UnityEngine;

namespace Player
{
    public enum MoveState
    {
        Stopped,
        Accelerating,
        Running,
        Decelerating,
        ChangingDirection,
        Dashing
    }
    
    [Serializable] public class PlayerHorizontalMovement
    {
        [Header("Ground Movement Stats")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private AnimationCurve _accelerationCurve;
        [SerializeField] [Range(0f, 1f)] private float _accelerationRate = 0.876f;
        [SerializeField] private float _accelerationTolerance = 0.005f;
        [SerializeField] private AnimationCurve _decelerationCurve;
        [SerializeField] [Range(0f, 1f)] private float _decelerationRate = 0.86f;
        [SerializeField] private float _decelerationTolerance = 0.005f;
        [SerializeField] private AnimationCurve _changeDirectionCurve;
        [SerializeField] [Range(0f, 1f)] private float _changeDirectionRate = 0.877f;
        [SerializeField] private float _changeDirectionTolerance = 0.005f;
        
        [Header("Air Move Stats")]
        [SerializeField] [Range(0f, 1f)] private float _airAccelerationRate = 0.282f;
        [SerializeField] [Range(0f, 1f)] private float _airDecelerationRate = 0.194f;
        [SerializeField] [Range(0f, 1f)] private float _airChangeDirectionRate = 0.736f;
        
        // TODO Could reformat entity block, or flow... need a nice way of modifying speed...
        // Either through bools or by setting...?
        // Bools could be more stable (like a state)
        private EntityBlock _entityBlock;
        public MoveState MoveState = MoveState.Stopped;
        private float _lerpCurrent = 0f;
        private float _baseMoveSpeed;

        public void MultiplyMoveSpeed(float multiple)
        {
            _moveSpeed = multiple * _baseMoveSpeed;
        }

        public void ResetMoveSpeed()
        {
            _moveSpeed = _baseMoveSpeed;
        }
        
        public void Initialise(EntityBlock entityBlock)
        {
            _entityBlock = entityBlock;
        }

        public void Start()
        {
            MoveState = MoveState.Stopped;
            _baseMoveSpeed = _moveSpeed;
        }

        public void Update(bool isMoveInput, Vector2 moveInput, ref Vector2 ref_playerVelocity, bool isCollisionBelow)
        {
            // Moving
            if (isMoveInput)
            {
                UpdateChangeDirection(moveInput, ref_playerVelocity);
                
                switch (MoveState)
                {
                    case MoveState.Stopped:
                        // NOTE: StartMoving() here is correct
                        // This is for if the player has stopped and an input comes in to start moving
                        StartMoving();
                        break;
                    case MoveState.Accelerating:
                        Accelerate(moveInput, ref ref_playerVelocity, isCollisionBelow);
                        break;
                    case MoveState.Decelerating:
                        // This will be called if the player starts decelerating and then wants to move again
                        // This means using "StartMoving()" here is correct.
                        StartMoving();
                        break;
                    case MoveState.Running:
                        Run(moveInput, ref ref_playerVelocity);
                        break;
                    case MoveState.ChangingDirection:
                        //changes the speed to the opposite one
                        ChangeDirection(moveInput, ref ref_playerVelocity, isCollisionBelow);
                        break;
                }
            }
            else
            {
                if (MoveState != MoveState.Stopped)
                {
                    StopMoving();
                    Decelerate(ref ref_playerVelocity, isCollisionBelow);
                }
            }
        }

        private void UpdateChangeDirection(Vector2 moveInput, Vector2 playerVelocity)
        {
            bool moveInputIsOppositeToVelocity = (int)Mathf.Sign(playerVelocity.x) != (int)moveInput.x;
            if (moveInputIsOppositeToVelocity && playerVelocity.x != 0)
            {
                StartChangingDirection();
            }
        }

        private void StartChangingDirection()
        {
            _lerpCurrent = 0f;
            MoveState = MoveState.ChangingDirection;
        }
        
        // Begins the movement, calls sets to accelerating
        private void StartMoving()
        {
            _lerpCurrent = 0f;
            MoveState = MoveState.Accelerating;
        }

        private void StopMoving()
        {
            _lerpCurrent = 0f;
            MoveState = MoveState.Decelerating;
        }
        
        // Accelerates to run speed, sets to moveState to run once at speed
        private void Accelerate(Vector2 moveInput, ref Vector2 ref_playerVelocity, bool isCollisionBelow)
        {
            //uses a lerp which is then used to evaluate along an animation curve for the acceleration
            //once we get to the max speed change to running
            //checks if there is a collision below the player, and if so use the air timers
            float rate = (isCollisionBelow ? _accelerationRate : _airAccelerationRate);
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            ref_playerVelocity.x = Mathf.Lerp(ref_playerVelocity.x, _moveSpeed * moveInput.x, _accelerationCurve.Evaluate(_lerpCurrent));
            
            if (_moveSpeed - Mathf.Abs(ref_playerVelocity.x) <=  _accelerationTolerance)
            {
                MoveState = MoveState.Running;
            }
        }
        
        // Continue moving at the current speed
        private void Run(Vector2 moveInput, ref Vector2 ref_playerVelocity)
        {
            float blockMoveSpeedModifier = 1f;
            if (_entityBlock && _entityBlock.IsBlocking())
            {
                blockMoveSpeedModifier = _entityBlock.blockSpeedModifier;
            }
            ref_playerVelocity.x = moveInput.x * _moveSpeed * blockMoveSpeedModifier;
        }

        private void Decelerate(ref Vector2 ref_playerVelocity, bool isCollisionBelow)
        {
            //same lerp method as accelerate
            //this time changes to stopped after getting low enough 
            //(I tried doing if(speed==0) but that was glitchy af
            float rate = isCollisionBelow ? _decelerationRate : _airDecelerationRate;
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            ref_playerVelocity.x = Mathf.Lerp(ref_playerVelocity.x, 0f, _decelerationCurve.Evaluate(_lerpCurrent));
            if (Mathf.Abs(ref_playerVelocity.x) <= _decelerationTolerance)
            {
                ref_playerVelocity.x = 0f;
                MoveState = MoveState.Stopped;
            }
        }
        
        private void ChangeDirection(Vector2 moveInput, ref Vector2 ref_playerVelocity, bool isCollisionBelow)
        {
            //same lerp method as accelerate
            float rate = isCollisionBelow ? _changeDirectionRate : _airChangeDirectionRate;
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            ref_playerVelocity.x = Mathf.Lerp(ref_playerVelocity.x, _moveSpeed * moveInput.x, _changeDirectionCurve.Evaluate(_lerpCurrent));

            if ((Mathf.Abs(ref_playerVelocity.x) - _moveSpeed) < _changeDirectionTolerance) 
            {
                MoveState = MoveState.Running;
            }
        }
    }
}