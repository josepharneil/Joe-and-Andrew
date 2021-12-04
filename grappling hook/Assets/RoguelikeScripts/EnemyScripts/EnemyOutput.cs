using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOutput : MonoBehaviour
{
    [Header("State")]
    [SerializeField] EnemyController enemyController;

    [Header("Outputs")]
    [SerializeField] EnemyPathing enemyPathing;

    [Header("Data needed to update")]
    private bool nothing_here_yet;


    // Update is called once per frame
    private void Update()
    {
        switch (enemyController.state)
        {
            case EnemyController.State.Patrolling:
                UpdatePatrolling();
                break;
            case EnemyController.State.ReturningToPatrol:
                UpdateReturnToPatrolling();
                break;
            case EnemyController.State.SeesPlayer:
                break;
            case EnemyController.State.ChasePlayer:
                UpdateChasePlayer();
                break;
            case EnemyController.State.AttackPlayer:
                break;
            case EnemyController.State.Dead:
                break;
            case EnemyController.State.Destroy:
                break;
        }
    }

    private void UpdatePatrolling()
    {
        enemyPathing.UpdatePatrol();
    }

    private void UpdateReturnToPatrolling()
    {
        enemyPathing.UpdateReturnToPatrol();
    }

    private void UpdateChasePlayer()
    {
        enemyPathing.UpdateChasePlayer();
    }
}
