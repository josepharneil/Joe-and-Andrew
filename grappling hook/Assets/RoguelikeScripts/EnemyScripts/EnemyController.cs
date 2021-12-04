using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyInput enemyInput;

    public enum State
    {
        Patrolling, // Patrolling can be stationary
        ReturningToPatrol, // Done chasing, now returns to patrol
        SeesPlayer, // Sees player, does nothing for now... (could be out of range)
        ChasePlayer, // Moves towards player
        AttackPlayer, // Attacks player when in range (combat)
        Dead, // Dead
        Destroy // Destroy the object I guess
    }
    public State state = State.Patrolling;

    public event Action<State> OnEnemyStateExited;
    public event Action<State> OnEnemyStateStarted;

    public void ChangeState( State newState )
    {
        if( state == newState)
        {
            return;
        }

        OnEnemyStateExited?.Invoke( state );

        state = newState;

        // TODO Exit current state?

        switch( newState )
        {
            case State.Patrolling:
                {
                    HandleChangeStatePatrolling();
                    break;
                }
            case State.ReturningToPatrol:
                {
                    HandleChangeStateReturningToPatrol();
                    break;
                }
            case State.SeesPlayer:
                {
                    HandleChangeStateSeesPlayer();
                    break;
                }
            case State.ChasePlayer:
                {
                    HandleChangeStateChasePlayer();
                    break;
                }
            case State.AttackPlayer:
                {
                    HandleChangeStateAttackPlayer();
                    break;
                }
            case State.Dead:
                {
                    HandleChangeStateDead();
                    break;
                }
            case State.Destroy:
                {
                    HandleChangeStateDestroy();
                    break;
                }
        }

        OnEnemyStateStarted?.Invoke( newState );
    }


    //=========================== Handlers ===========================//

    private void HandleChangeStatePatrolling()
    {

    }

    private void HandleChangeStateReturningToPatrol()
    {

    }

    private void HandleChangeStateSeesPlayer()
    {

    }

    private void HandleChangeStateChasePlayer()
    {

    }

    private void HandleChangeStateAttackPlayer()
    {

    }

    private void HandleChangeStateDead()
    {

    }

    private void HandleChangeStateDestroy()
    {

    }

    //=========================== Handlers ===========================//

    private void Update()
    {
        switch (state)
        {
            case State.Patrolling:
                {
                    CheckRaycastToPlayer();
                    CheckIfDead();
                    break;
                }
            case State.ReturningToPatrol:
                {
                    CheckRaycastToPlayer();
                    if(enemyInput.IsAtOriginalPosition)
                    {
                        ChangeState(State.Patrolling);
                    }
                    CheckIfDead();
                    break;
                }
            case State.SeesPlayer:
                {
                    CheckRaycastToPlayer();
                    CheckIfDead();
                    break;
                }
            case State.ChasePlayer:
                {
                    CheckRaycastToPlayer();
                    CheckIfDead();
                    break;
                }
            case State.AttackPlayer:
                {
                    CheckRaycastToPlayer();
                    CheckIfDead();
                    break;
                }
            case State.Dead:
                {
                    CheckIfAlive();
                    break;
                }
            case State.Destroy:
                {
                    break;
                }
        }
    }

    private void CheckRaycastToPlayer()
    {
        if( !enemyInput.IsRaycastHittingPlayer() )
        {
            return;
        }

        // Check within attack range.
        if (enemyInput.IsInAttackRange())
        {
            ChangeState(State.AttackPlayer);
        }
        // Check within chase distance
        else if (enemyInput.IsInChaseRange())
        {
            ChangeState(State.ChasePlayer);
        }
        else if (enemyInput.IsInSightRange())
        {
            ChangeState(State.SeesPlayer);
        }
        else
        {
            // If we're not already patrolling
            if( state != State.Patrolling )
            {
                ChangeState(State.ReturningToPatrol);
            }
        }
    }

    private void CheckIfDead()
    {
        if( enemyInput.enemyHealth.IsDead() )
        {
            ChangeState(State.Dead);
        }
    }

    private void CheckIfAlive()
    {
        if( enemyInput.enemyHealth.IsAlive() )
        {
            ChangeState(State.Patrolling);
        }
    }

}
