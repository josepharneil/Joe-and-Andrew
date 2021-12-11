using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{
    //[SerializeField] private PlayerMovement movement;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerAttack _playerAttack;
    
    // private enum PlayerState
    // {
    //     Movement, // In movement mode
    //     Attacking, // In attack mode, can't move
    //     Dead
    // }
    // private PlayerState state;
    private StateMachine _stateMachine;

    private void Awake()
    {
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();
        
        // Define states
        //var movementState = new MovementState();
       // var rollState = new RollState();
        //var attackState = new AttackState();
        
        // Predicates
        // Nothing for now, just staying in movement state.
        bool IsAttacking() => _playerAttack.isAttacking;
        bool IsNotAttacking() => !_playerAttack.isAttacking;

        // Transitions
        _stateMachine.AddTransition(_playerMovement, _playerAttack, IsAttacking);
        _stateMachine.AddTransition(_playerAttack, _playerMovement, IsNotAttacking);
        
        // Initial state
        _stateMachine.SetState( _playerMovement );
        
    }

    // Update is called once per frame
    private void Update() => _stateMachine.Tick();
    /*{
        switch (state)
        {
            case PlayerState.Movement:
                {
                    movement.MovementUpdate();
                    break;
                }
            case PlayerState.Attacking:
                break;
            case PlayerState.Dead:
                break;
        }
    }
    */

    private void FixedUpdate() => _stateMachine.FixedTick();
    // {
    //     switch (state)
    //     {
    //         case PlayerState.Movement:
    //         {
    //             movement.MovementFixedUpdate();
    //             break;
    //         }
    //         case PlayerState.Attacking:
    //             break;
    //         case PlayerState.Dead:
    //             break;
    //     }
    // }
}
