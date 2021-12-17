using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class EnemyController : MonoBehaviour
{
    private StateMachine _stateMachine;

    [Header("Inputs")]
    [SerializeField] private EnemyInput input;
    [SerializeField] public EnemyAttackPlayer attackPlayerState;

    [Header("Output")]
    [SerializeField] private EnemyPathing pathing;

    [Header("Debug")]
    [SerializeField] private bool debugShowStateName = true;
    private void OnDrawGizmos()
    {
        if(debugShowStateName && _stateMachine != null)
        {
            // todo encapsulate just for name
            string stateText = _stateMachine.GetCurrentStateTypeName();

            GUIStyle customStyle = new GUIStyle();
            customStyle.fontSize = 14;   // can also use e.g. <size=30> in Rich Text
            customStyle.richText = true;
            Vector3 textPosition = transform.position + (Vector3.up * 0.3f);
            string richText = "<color=red><B>State: " + stateText + "</B></color>";

            Handles.Label(textPosition, richText, customStyle);
        }
    }

    private void Awake()
    {
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();

        // Define states
        var patrollingState = new EnemyPatrolling();
        var returningToPatrolState = new EnemyReturnToPatrol( pathing );
        var seesPlayerState = new EnemySeesPlayer();
        var chasePlayerState = new EnemyChasePlayer( pathing );
        var deadState = new EnemyDead();

        // Predicates
        // This is based on if !sight_range, then !chase && !attack
        bool IsInAttackRange() => input.PlayerIsInAttackRange();
        bool IsInChaseRange() => input.PlayerIsInChaseRange();
        bool IsInSightRange() => input.PlayerIsInSightRange();

        bool IsNotInSightRange() => !input.PlayerIsInSightRange();
        // Func<bool> isNotInChaseRange = () => !input.PlayerIsInChaseRange();
        // Func<bool> IsNotInAttackRange = () => !input.PlayerIsInAttackRange();

        bool IsNotInAttackRangeAndIsInChaseRange() => !input.PlayerIsInAttackRange() && input.PlayerIsInChaseRange();
        bool IsNotInChaseRangeAndIsInSightRange() => !input.PlayerIsInChaseRange() && input.PlayerIsInSightRange();

        // Func<bool> ShouldReturnToPatrolling = () => !input.PlayerIsInSightRange();

        bool IsBackToPatrolling() => input.IsAtOriginalPosition;

        bool IsDead() => input.enemyHealth.IsDead();
        bool IsAlive() => input.enemyHealth.IsAlive();


        // Add transitions
        // Patrolling state
        _stateMachine.AddTransition(patrollingState, seesPlayerState, IsInSightRange);

        // Sees player state
        _stateMachine.AddTransition(seesPlayerState, chasePlayerState, IsInChaseRange);
        _stateMachine.AddTransition(seesPlayerState, returningToPatrolState, IsNotInSightRange);

        // Chase player state
        _stateMachine.AddTransition(chasePlayerState, attackPlayerState, IsInAttackRange);
        _stateMachine.AddTransition(chasePlayerState, seesPlayerState, IsNotInChaseRangeAndIsInSightRange);
        _stateMachine.AddTransition(chasePlayerState, returningToPatrolState, IsNotInSightRange);

        // Attacking state
        _stateMachine.AddTransition(attackPlayerState, returningToPatrolState, IsNotInSightRange);
        _stateMachine.AddTransition(attackPlayerState, chasePlayerState, IsNotInAttackRangeAndIsInChaseRange);
        _stateMachine.AddTransition(attackPlayerState, seesPlayerState, IsNotInChaseRangeAndIsInSightRange);

        // Return to patrolling state
        _stateMachine.AddTransition(returningToPatrolState, patrollingState, IsBackToPatrolling);
        _stateMachine.AddTransition(returningToPatrolState, seesPlayerState, IsInSightRange);
        _stateMachine.AddTransition(returningToPatrolState, chasePlayerState, IsInChaseRange);
        _stateMachine.AddTransition(returningToPatrolState, attackPlayerState, IsInAttackRange);

        // Dead state
        _stateMachine.AddAnyTransition(deadState, IsDead);
        _stateMachine.AddTransition(deadState, patrollingState, IsAlive);

        // Set initial state.
        _stateMachine.SetState(patrollingState);
    }

    private void Update() => _stateMachine.Tick();


}
