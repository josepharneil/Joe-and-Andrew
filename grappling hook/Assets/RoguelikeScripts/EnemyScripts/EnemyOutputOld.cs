using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOutputOld : MonoBehaviour
{
    [Header("State")]
    [SerializeField] EnemyControllerOld enemyController;

    [Header("Outputs")]
    [SerializeField] EnemyPathing enemyPathing;

    [Header("Data needed to update")]
    private bool nothing_here_yet;


    // Update is called once per frame
    private void Update()
    {
        switch (enemyController.state)
        {
            case EnemyControllerOld.State.Patrolling:
                UpdatePatrolling();
                break;
            case EnemyControllerOld.State.ReturningToPatrol:
                UpdateReturnToPatrolling();
                break;
            case EnemyControllerOld.State.SeesPlayer:
                break;
            case EnemyControllerOld.State.ChasePlayer:
                UpdateChasePlayer();
                break;
            case EnemyControllerOld.State.AttackPlayer:
                break;
            case EnemyControllerOld.State.Dead:
                break;
            case EnemyControllerOld.State.Destroy:
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
