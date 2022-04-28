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
        [SerializeField] private bool _attacksDrivenByAnimations = true;
        public PlayerAttackDriver PlayerAttackDriver;

        [Header("Downwards Attack Jump")] 
        [SerializeField] private float _downAttackJumpVelocity = 15f;

        [NonSerialized] public bool IsAttacking = false;
        [NonSerialized] public bool IsInPreDamageAttackPhase = true;
        [NonSerialized] public AttackDirection AttackDirection;

        public void Initialise(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            PlayerAttackDriver.Initialise(_playerCombat);
        }

        public void ShowGUI()
        {
            PlayerAttackDriver.ShowDebugGUI();
        }
        
        public void Update(DashState dashState, PlayerAnimator playerAnimator, bool isMoveInput, bool isJumpInput)
        {
            UpdateAttackDriver();
            CheckIfAttackIsCancellable(dashState, playerAnimator, isMoveInput, isJumpInput);
        }

        public void StartAttack(PlayerAnimator playerAnimator, bool isGrounded, Vector2 moveInput, ref FacingDirection facingDirection)
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
                        playerAnimator.SetSpriteFlipX(true);
                    }
                    else if ( moveInput.x > 0 )
                    {
                        facingDirection = FacingDirection.Right;
                        playerAnimator.SetSpriteFlipX(false);
                    }
                }
                AttackDirection = (facingDirection == FacingDirection.Left) ? AttackDirection.Left : AttackDirection.Right;
            }
            
            if (_attacksDrivenByAnimations)
            {
                playerAnimator.SetTriggerAttack();
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

        private void CheckIfAttackIsCancellable(DashState dashState, PlayerAnimator playerAnimator, bool isMoveInput, bool isJumpInput)
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
                    playerAnimator.PlayState("Player_Idle");
                    // todo getting playercombat here is bad.
                    _playerCombat.ForceHideAttackParticles();
                }
            }

            if ((PlayerCombatPrototyping.data.cancellables & PrototypeCancellables.Jump) != PrototypeCancellables.None) 
            {
                if (isJumpInput)
                {
                    IsAttacking = false;
                    playerAnimator.PlayState("Player_Jump");
                    _playerCombat.ForceHideAttackParticles();
                }
            }
                
            if ((PlayerCombatPrototyping.data.cancellables & PrototypeCancellables.Movement) != PrototypeCancellables.None) 
            {
                if (isMoveInput)
                {
                    IsAttacking = false;
                    playerAnimator.PlayState("Player_Idle");
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