using System;
using Entity;
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
        
        private PlayerInputs _playerInputs;
        private MovementController _movementController;
        private EntityBlock _entityBlock;
        public MoveState MoveState = MoveState.Stopped;
        private float _lerpCurrent = 0f;

        public void SetMoveSpeed(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
        }
        
        public void Initialise(PlayerInputs playerInputs, MovementController movementController, EntityBlock entityBlock)
        {
            _playerInputs = playerInputs;
            _movementController = movementController;
            _entityBlock = entityBlock;
        }

        public void Start()
        {
            MoveState = MoveState.Stopped;
        }

        public void Update()
        {
            bool isMoveInput = _playerInputs.GetIsMoveInput();
            
            // Moving
            if (isMoveInput)
            {
                UpdateChangeDirection();
                
                switch (MoveState)
                {
                    case MoveState.Stopped:
                        //begins the movement, calls sets to accelerating
                        //todo Joe here, is this correct? Start moving in the stopped state?
                        //AK: yeah this is correct, this is for if the player has stopped and an input comes in to start moving
                        StartMoving();
                        break;
                    case MoveState.Accelerating:
                        //accelerates to run speed, sets to moveState to run once at speed
                        Accelerate();
                        break;
                    case MoveState.Decelerating:
                        //this will be called if the player starts decelerating and then wants to move again
                        StartMoving();
                        break;
                    case MoveState.Running:
                        //continues moving at the current speed
                        Run();
                        break;
                    case MoveState.ChangingDirection:
                        //changes the speed to the opposite one
                        ChangeDirection();
                        break;
                }
            }
            else
            {
                if (MoveState != MoveState.Stopped)
                {
                    StopMoving();
                    Decelerate();
                }
            }
        }

        private void UpdateChangeDirection()
        {
            Vector2 playerVelocity = _playerInputs.Velocity;
            Vector2 moveInput = _playerInputs.GetMoveInput();

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
        
        private void Accelerate()
        {
            //uses a lerp which is then used to evaluate along an animation curve for the acceleration
            //once we get to the max speed change to running
            //checks if there is a collision below the player, and if so use the air timers
            float rate = (_movementController.customCollider2D.CollisionBelow ? _accelerationRate : _airAccelerationRate);
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            _playerInputs.Velocity.x = Mathf.Lerp(_playerInputs.Velocity.x, _moveSpeed * _playerInputs.GetMoveInput().x, _accelerationCurve.Evaluate(_lerpCurrent));
            
            if (_moveSpeed - Mathf.Abs(_playerInputs.Velocity.x) <=  _accelerationTolerance)
            {
                MoveState = MoveState.Running;
            }
        }
        
        private void Run()
        {
            float blockMoveSpeedModifier = 1f;
            if (_entityBlock && _entityBlock.IsBlocking())
            {
                blockMoveSpeedModifier = _entityBlock.blockSpeedModifier;
            }
            _playerInputs.Velocity.x = _playerInputs.GetMoveInput().x * _moveSpeed * blockMoveSpeedModifier;
        }

        private void Decelerate()
        {
            //same lerp method as accelerate
            //this time changes to stopped after getting low enough 
            //(I tried doing if(speed==0) but that was glitchy af
            float rate = _movementController.customCollider2D.CollisionBelow ? _decelerationRate : _airDecelerationRate;
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            _playerInputs.Velocity.x = Mathf.Lerp(_playerInputs.Velocity.x, 0f, _decelerationCurve.Evaluate(_lerpCurrent));
            if (Mathf.Abs(_playerInputs.Velocity.x) <= _decelerationTolerance)
            {
                _playerInputs.Velocity.x = 0f;
                MoveState = MoveState.Stopped;
            }
        }
        
        private void ChangeDirection()
        {
            //same lerp method as accelerate
            float rate = _movementController.customCollider2D.CollisionBelow ? _changeDirectionRate : _airChangeDirectionRate;
            _lerpCurrent = Mathf.Lerp(_lerpCurrent, 1f, rate * Time.deltaTime);
            _playerInputs.Velocity.x = Mathf.Lerp(_playerInputs.Velocity.x, _moveSpeed * _playerInputs.GetMoveInput().x, _changeDirectionCurve.Evaluate(_lerpCurrent));

            if ((Mathf.Abs(_playerInputs.Velocity.x) - _moveSpeed) < _changeDirectionTolerance) 
            {
                MoveState = MoveState.Running;
            }
        }
    }
}