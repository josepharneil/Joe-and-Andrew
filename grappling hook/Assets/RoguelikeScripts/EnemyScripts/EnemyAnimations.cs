using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private EnemyController enemyController;

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

    private void OnEnemyStateStarted( EnemyController.State newState )
    {
        switch (newState)
        {
            case EnemyController.State.Patrolling:
                spriteRenderer.color = Color.green;
                break;
            case EnemyController.State.ReturningToPatrol:
                spriteRenderer.color = Color.yellow;
                break;
            case EnemyController.State.SeesPlayer:
                spriteRenderer.color = Color.cyan;
                break;
            case EnemyController.State.ChasePlayer:
                spriteRenderer.color = Color.red;
                break;
            case EnemyController.State.AttackPlayer:
                spriteRenderer.color = Color.black;
                break;
            case EnemyController.State.Dead:
                spriteRenderer.color = Color.gray;
                break;
            case EnemyController.State.Destroy:
                break;
        }
    }
}
