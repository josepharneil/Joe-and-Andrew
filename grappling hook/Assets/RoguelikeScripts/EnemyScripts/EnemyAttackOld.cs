using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAttackOld : MonoBehaviour
{
    [Header("Stats")]
    public int AttackDamage = 5;
    public float ParryTimeLimit = 0.2f; // Time frame for player to parry.
    [SerializeField] private float attackWindUpTime = 0.2f;
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float timeBetweenAttackAndRecovery = 0.2f;
    [SerializeField] private float attackRecoveryTime = 1f;

    
    [SerializeField] private float parryRecoilTime = 0.7f;
    [SerializeField] private float stunTimeFromParry = 1.5f;

    [Header("Weapon Components")]
    [SerializeField] private Transform parentTransform;

    [Header("Components")]
    [SerializeField] EnemyControllerOld enemyController;

    private Quaternion initialParentRotation; // So we can reset to this.

    public bool IsInDamageDealingPhase = false; // Not true if for example the sword is going back up.


    private Sequence attackSequence;

    private void Awake()
    {
        initialParentRotation = parentTransform.rotation;

        attackSequence = DOTween.Sequence();

        // Set up attack sequence
        attackSequence.AppendInterval(attackWindUpTime);

        attackSequence.AppendCallback(() =>
        {
            IsInDamageDealingPhase = true;
        });

        attackSequence.Append(
            parentTransform.DORotate(
               endValue: new Vector3(0, 0, 130),
               duration: attackDuration,
               mode: RotateMode.WorldAxisAdd)
                    .SetEase(Ease.InOutBack)
                    .OnComplete( () =>
                    {
                        IsInDamageDealingPhase = false;
                    }));

        attackSequence.AppendInterval(timeBetweenAttackAndRecovery);

        attackSequence.Append(
            parentTransform.DORotate(
                endValue: initialParentRotation.eulerAngles,
                duration: attackRecoveryTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.InOutBack));


        // For now, just infinite loops.
        attackSequence.SetLoops(-1, LoopType.Restart);

        attackSequence.Pause();
    }

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
            case EnemyControllerOld.State.AttackPlayer:
                StartAttacking();
                break;
            case EnemyControllerOld.State.Patrolling:
            case EnemyControllerOld.State.ReturningToPatrol:
            case EnemyControllerOld.State.SeesPlayer:
            case EnemyControllerOld.State.ChasePlayer:
            case EnemyControllerOld.State.Dead:
            case EnemyControllerOld.State.Destroy:
                StopAttacking();
                WindBackAttack();
                break;
        }
    }

    private void StartAttacking()
    {
        parentTransform.rotation = initialParentRotation;
        IsInDamageDealingPhase = true;
        attackSequence.Restart();
        attackSequence.Play();
    }

    private void StopAttacking()
    {
        IsInDamageDealingPhase = false;
        //attackSequence.Restart();
        attackSequence.Pause();
    }

    private void WindBackAttack()
    {
        parentTransform.DORotate(
                endValue: initialParentRotation.eulerAngles,
                duration: attackRecoveryTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.InOutBack);
    }

    public void Parried()
    {
        // TODO Maybe this should be part of a sub-state machine for attacking... idk
        // But parry affects animations, so maybe feed into input idk
        StopAttacking();

        Sequence parrySequence = DOTween.Sequence();

        parrySequence.Append(
            parentTransform.DORotate(
                endValue: initialParentRotation.eulerAngles,
                duration: parryRecoilTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.OutQuint));

        parrySequence.AppendInterval(stunTimeFromParry);

        parrySequence.OnComplete(
            () =>
            {
                StartAttacking();
            });
    }
}
