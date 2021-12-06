using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class EnemyController : MonoBehaviour
{
    private StateMachine stateMachine;

    [Header("Inputs")]
    [SerializeField] EnemyInput input;
    public EnemyAttackData attackData;

    [Header("Output")]
    [SerializeField] private EnemyPathing pathing;

    //public enum State
    //{
    //    Patrolling, // Patrolling can be stationary
    //    ReturningToPatrol, // Done chasing, now returns to patrol
    //    SeesPlayer, // Sees player, does nothing for now... (could be out of range)
    //    ChasePlayer, // Moves towards player
    //    AttackPlayer, // Attacks player when in range (combat)
    //    Dead, // Dead
    //    Destroy // Destroy the object I guess
    //}

    [Header("Debug")]
    [SerializeField] private bool debugShowStateName = true;
    private void OnDrawGizmos()
    {
        if(debugShowStateName && stateMachine != null)
        {
            // todo encapsulate just for name
            string stateText = stateMachine.GetCurrentStateTypeName();

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
        stateMachine = new StateMachine();

        // Define states
        var patrollingState = new EnemyPatrolling();
        var returningToPatrolState = new EnemyReturnToPatrol( pathing );
        var seesPlayerState = new EnemySeesPlayer();
        var chasePlayerState = new EnemyChasePlayer( pathing );
        var attackPlayerState = new EnemyAttackPlayer( ref attackData );
        var deadState = new EnemyDead();

        // Predicates
        // This is based on if !sight_range, then !chase && !attack
        Func<bool> IsInAttackRange = () => input.PlayerIsInAttackRange();
        Func<bool> IsInChaseRange = () => input.PlayerIsInChaseRange();
        Func<bool> IsInSightRange = () => input.PlayerIsInSightRange();

        Func<bool> IsNotInSightRange = () => !input.PlayerIsInSightRange();
        Func<bool> IsNotInChaseRange = () => !input.PlayerIsInChaseRange();
        Func<bool> IsNotInAttackRange = () => !input.PlayerIsInAttackRange();

        Func<bool> IsNotInAttackRangeAndIsInChaseRange = () => !input.PlayerIsInAttackRange() && input.PlayerIsInChaseRange();
        Func<bool> IsNotInChaseRangeAndIsInSightRange = () => !input.PlayerIsInChaseRange() && input.PlayerIsInSightRange();

        Func<bool> ShouldReturnToPatrolling = () => !input.PlayerIsInSightRange();

        Func<bool> IsBackToPatrolling = () => input.IsAtOriginalPosition;

        Func<bool> IsDead = () => input.enemyHealth.IsDead();
        Func<bool> IsAlive = () => input.enemyHealth.IsAlive();


        // Add transitions
        // Patrolling state
        stateMachine.AddTransition(patrollingState, seesPlayerState, IsInSightRange);

        // Sees player state
        stateMachine.AddTransition(seesPlayerState, chasePlayerState, IsInChaseRange);
        stateMachine.AddTransition(seesPlayerState, returningToPatrolState, IsNotInSightRange);

        // Chase player state
        stateMachine.AddTransition(chasePlayerState, attackPlayerState, IsInAttackRange);
        stateMachine.AddTransition(chasePlayerState, seesPlayerState, IsNotInChaseRangeAndIsInSightRange);
        stateMachine.AddTransition(chasePlayerState, returningToPatrolState, IsNotInSightRange);

        // Attacking state
        stateMachine.AddTransition(attackPlayerState, returningToPatrolState, IsNotInSightRange);
        stateMachine.AddTransition(attackPlayerState, chasePlayerState, IsNotInAttackRangeAndIsInChaseRange);
        stateMachine.AddTransition(attackPlayerState, seesPlayerState, IsNotInChaseRangeAndIsInSightRange);

        // Return to patrolling state
        stateMachine.AddTransition(returningToPatrolState, patrollingState, IsBackToPatrolling);
        stateMachine.AddTransition(returningToPatrolState, seesPlayerState, IsInSightRange);
        stateMachine.AddTransition(returningToPatrolState, chasePlayerState, IsInChaseRange);
        stateMachine.AddTransition(returningToPatrolState, attackPlayerState, IsInAttackRange);

        // Dead state
        stateMachine.AddAnyTransition(deadState, IsDead);
        stateMachine.AddTransition(deadState, patrollingState, IsAlive);

        // Set initial state.
        stateMachine.SetState(patrollingState);
    }

    private void Update() => stateMachine.Tick();


}
