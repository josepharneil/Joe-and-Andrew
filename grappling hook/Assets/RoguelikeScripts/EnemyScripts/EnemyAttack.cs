using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAttack : MonoBehaviour
{
    public int AttackDamage = 1;
    public float ParryTimeLimit = 0.2f;

    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private Transform parent;
    private Vector3 initialRotation;

    public bool IsAttacking = true; // Not true if for example the sword is going back up.

    Tween swordSwingTween;

    private void OnEnable()
    {
        EnemyMovement.OnEnemyMovementStateChanged += EnemyMovementOnMovementStateChanged;
    }
    private void OnDisable()
    { 
        EnemyMovement.OnEnemyMovementStateChanged -= EnemyMovementOnMovementStateChanged;
    }

    private void Awake()
    {
        SwingSwordTween();
        swordSwingTween.Pause();
    }

    private void EnemyMovementOnMovementStateChanged( EnemyMovement.EnemyMovementState state)
    {
        switch( state )
        {
            case EnemyMovement.EnemyMovementState.Searching:
                {
                    StopEnemyAttack();
                    break;
                }
            case EnemyMovement.EnemyMovementState.Moving:
                {
                    StopEnemyAttack();
                    break;
                }
            case EnemyMovement.EnemyMovementState.Attacking:
                {
                    StopEnemyAttack();
                    SwingSwordTween();
                    break;
                }
        }
    }

    private void Start()
    {
        initialRotation = parent.rotation.eulerAngles;
    }

    public void StopEnemyAttack()
    {
        swordSwingTween.Restart();
        swordSwingTween.Pause();
    }

    public void ParryTween()
    {
        IsAttacking = false;
        swordSwingTween.Pause();
        ParriedSwordTween();
    }

    private void SwingSwordTween()
    {
        swordSwingTween = parent.DORotate(
               endValue: new Vector3(0, 0, 130),
               duration: attackDuration,
               mode: RotateMode.WorldAxisAdd).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Restart);
    }

    private void ParriedSwordTween()
    {
        parent.DORotate(
               endValue: initialRotation,
               duration: attackDuration / 2f,
               mode: RotateMode.Fast)
            .SetEase(Ease.OutQuint)
            .OnComplete(
            () =>
            {
                swordSwingTween.Restart();
                IsAttacking = true;
            } );
    }
}
