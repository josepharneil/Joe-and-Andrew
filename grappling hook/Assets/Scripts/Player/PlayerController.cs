using System;
using Entity;
using Physics;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(BoxRayCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] public PlayerMovement PlayerMovement;
        [Header("Attacking")] public PlayerAttacks PlayerAttacks;
        [Header("Inputs")] public PlayerInputs PlayerInputs;
        [Header("Animator")] [SerializeField] private PlayerAnimator _playerAnimator;
        [Header("Player Sounds")] [SerializeField] private PlayerSounds _playerSounds;
        
        [Header("Entity Components")] 
        [SerializeField] private EntityParry _entityParry;
        [SerializeField] private EntityBlock _entityBlock;
        [SerializeField] private EntityKnockback _entityKnockback;
        [SerializeField] private EntityDaze _entityDaze;

        private void Awake()
        {
            PlayerInputs.Initialise(this, _playerAnimator, _entityParry, _entityBlock, _entityKnockback, _entityDaze);
            PlayerMovement.Initialise(_playerAnimator, _entityBlock, _playerSounds);
            PlayerAttacks.Initialise(transform, PlayerMovement, _playerAnimator);
        }

        private void Start()
        {
            PlayerMovement.Start();
            _playerAnimator.Start();
            _playerSounds.Start();
        }
        
        private void OnGUI()
        {
            PlayerAttacks.ShowGUI();
        }

        private void OnDrawGizmosSelected()
        {
            PlayerAttacks.DrawGizmosSelected();
        }

        private void Update()
        {
            // Input (could put this in an input function???)
            PlayerInputs.UpdateInputs();
            
            // Movement
            PlayerMovement.Update(
                ref PlayerInputs.IsMoveInput, 
                ref PlayerInputs.MoveInput, 
                ref PlayerInputs.IsJumpInput, 
                ref PlayerInputs.IsBufferedJumpInput, 
                ref PlayerInputs.IsJumpEndedEarly, 
                PlayerInputs.JumpInputTime, 
                PlayerAttacks.IsAttacking,
                PlayerAttacks.PlayerCombatPrototyping);

            // Attacks
            PlayerAttacks.Update(PlayerMovement.PlayerDash.DashState, PlayerInputs.IsMoveInput, PlayerInputs.IsJumpInput);
        }
    }
}