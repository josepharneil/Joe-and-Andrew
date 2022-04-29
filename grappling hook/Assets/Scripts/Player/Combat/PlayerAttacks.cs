using System;
using Entity;
using UnityEngine;

namespace Player
{
    [Serializable] public class PlayerAttacks
    {
        [Header("Components")]
        public PlayerEquipment CurrentPlayerEquipment;
        public PlayerCombatPrototyping PlayerCombatPrototyping;
        [SerializeField] private PlayerCombat _playerCombat;
        PlayerMovement _playerMovement;

        [Header("Attack Driver")]
        [SerializeField] private bool _attacksDrivenByAnimations = false;
        public PlayerAttackDriver PlayerAttackDriver;

        [Header("Downwards Attack Jump")] 
        [SerializeField] private float _downAttackJumpVelocity = 15f;

        [NonSerialized] public bool IsAttacking = false;
        [NonSerialized] public bool IsInPreDamageAttackPhase = true;
        [NonSerialized] public AttackDirection AttackDirection;

        private PlayerAnimator _playerAnimator;

        public void Initialise(Transform playerTransform, PlayerMovement playerMovement, PlayerAnimator playerAnimator)
        {
            _playerCombat.Initialise(playerTransform, this);
            PlayerAttackDriver.Initialise(_playerCombat);
            _playerMovement = playerMovement;
            _playerAnimator = playerAnimator;
        }

        public void ShowGUI()
        {
            PlayerAttackDriver.ShowDebugGUI();
        }

        public void DrawGizmosSelected()
        {
            _playerCombat.DrawGizmosSelected();
        }
        
        public void Update(DashState dashState, bool isMoveInput, bool isJumpInput)
        {
            UpdateAttackDriver();
            CheckIfAttackIsCancellable(dashState, isMoveInput, isJumpInput);
            _playerCombat.Update();
        }

        public void StartAttack(bool isGrounded, Vector2 moveInput, ref FacingDirection facingDirection)
        {
            const float verticalInputThreshold = 0.5f;

            if (moveInput.y > verticalInputThreshold)
            {
                AttackDirection = AttackDirection.Up;
            }
            else if (!isGrounded && (moveInput.y < -verticalInputThreshold))
            {
                AttackDirection = AttackDirection.Down;
            }
            else
            {
                if (PlayerCombatPrototyping.data.canChangeDirectionsDuringAttack)
                {
                    if (moveInput.x < 0)
                    {
                        facingDirection = FacingDirection.Left;
                        _playerAnimator.SetSpriteFlipX(true);
                    }
                    else if ( moveInput.x > 0 )
                    {
                        facingDirection = FacingDirection.Right;
                        _playerAnimator.SetSpriteFlipX(false);
                    }
                }
                AttackDirection = (facingDirection == FacingDirection.Left) ? AttackDirection.Left : AttackDirection.Right;
            }
            
            if (_attacksDrivenByAnimations)
            {
                _playerAnimator.SetTriggerAttack();
            }
            else
            {
                PlayerAttackDriver.StartAttack();
            }
        }

        private void UpdateAttackDriver()
        {
            if (!_attacksDrivenByAnimations)
            {
                PlayerAttackDriver.UpdateAttack();
            }
        }

        private void CheckIfAttackIsCancellable(DashState dashState, bool isMoveInput, bool isJumpInput)
        {
            // Cancellable attack phases
            if (!IsAttacking) return;

            // TODO There are only really two phases right now
            // the actual attack phase is only 1 frame right now.
            if (IsInPreDamageAttackPhase)
            {
                // What phases are cancellable?
                if ((PlayerCombatPrototyping.data.cancellableAttackPhases &
                     PrototypeAttackPhases.PreDamage) == PrototypeAttackPhases.None)
                {
                    return;
                }
            }
            else // Post damage
            {
                if ((PlayerCombatPrototyping.data.cancellableAttackPhases &
                     PrototypeAttackPhases.PostDamage) == PrototypeAttackPhases.None)
                {
                    return;
                }
            }
            
            // What cancels attacks?
            if ((PlayerCombatPrototyping.data.cancellables & PrototypeCancellables.Dash) != PrototypeCancellables.None)
            {
                if (dashState == DashState.StartDash)
                {
                    IsAttacking = false;
                    _playerAnimator.PlayState("Player_Idle");
                    // todo getting playercombat here is bad.
                    _playerCombat.ForceHideAttackParticles();
                }
            }

            if ((PlayerCombatPrototyping.data.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
            {
                if (isJumpInput)
                {
                    IsAttacking = false;
                    _playerAnimator.PlayState("Player_Jump");
                    _playerCombat.ForceHideAttackParticles();
                }
            }
                
            if ((PlayerCombatPrototyping.data.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
            {
                if (isMoveInput)
                {
                    IsAttacking = false;
                    _playerAnimator.PlayState("Player_Idle");
                    _playerCombat.ForceHideAttackParticles();
                }
            }
        }

        public void DownwardsAttackJump()
        {
            _playerMovement.Velocity.y = _downAttackJumpVelocity;
        }
    }
}