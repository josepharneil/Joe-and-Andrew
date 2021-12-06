using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationsOld : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private EnemyControllerOld enemyController;

    [Header("Output")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        enemyController.OnEnemyStateStarted += OnEnemyStateStarted;
    }

    private void OnDisable()
    {
        enemyController.OnEnemyStateStarted -= OnEnemyStateStarted;
    }

    private void OnEnemyStateStarted( EnemyControllerOld.State newState )
    {
        switch (newState)
        {
            case EnemyControllerOld.State.Patrolling:
                spriteRenderer.color = Color.green;
                break;
            case EnemyControllerOld.State.ReturningToPatrol:
                spriteRenderer.color = Color.yellow;
                break;
            case EnemyControllerOld.State.SeesPlayer:
                spriteRenderer.color = Color.cyan;
                break;
            case EnemyControllerOld.State.ChasePlayer:
                spriteRenderer.color = Color.red;
                break;
            case EnemyControllerOld.State.AttackPlayer:
                spriteRenderer.color = Color.black;
                break;
            case EnemyControllerOld.State.Dead:
                spriteRenderer.color = Color.gray;
                break;
            case EnemyControllerOld.State.Destroy:
                break;
        }
    }
}
