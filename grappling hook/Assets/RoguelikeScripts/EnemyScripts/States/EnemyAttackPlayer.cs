using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAttackPlayer : IState
{
    public EnemyAttackData attackData;
    public EnemyAttackPlayer( ref EnemyAttackData data )
    {
        this.attackData = data;

        Init();
    }

    public void OnEnter()
    {
        StartAttacking();
    }

    public void Tick()
    {
        //throw new System.NotImplementedException();

        if(attackData.Parried)
        {
            Parried();
            attackData.Parried = false;
        }
    }
    
    public void FixedTick()
    {
    }

    public void OnExit()
    {
        StopAttacking();
        WindBackAttack();
    }

    // TODO This could be a state machine too.

    private Quaternion initialParentRotation; // So we can reset to this.

    private Sequence attackSequence;

    private void Init()
    {
        initialParentRotation = attackData.parentWeaponTransform.rotation;

        attackSequence = DOTween.Sequence();

        // Set up attack sequence
        attackSequence.AppendInterval(attackData.attackWindUpTime);

        attackSequence.AppendCallback(() =>
        {
            attackData.IsInDamageDealingPhase = true;
        });

        attackSequence.Append(
            attackData.parentWeaponTransform.DORotate(
               endValue: new Vector3(0, 0, 130),
               duration: attackData.attackDuration,
               mode: RotateMode.WorldAxisAdd)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        attackData.IsInDamageDealingPhase = false;
                    }));

        attackSequence.AppendInterval(attackData.timeBetweenAttackAndRecovery);

        attackSequence.Append(
            attackData.parentWeaponTransform.DORotate(
                endValue: initialParentRotation.eulerAngles,
                duration: attackData.attackRecoveryTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.InOutBack));


        // For now, just infinite loops.
        attackSequence.SetLoops(-1, LoopType.Restart);

        attackSequence.Pause();
    }

    private void StartAttacking()
    {
        attackData.parentWeaponTransform.rotation = initialParentRotation;
        attackData.IsInDamageDealingPhase = true;
        attackSequence.Restart();
        attackSequence.Play();
    }

    private void StopAttacking()
    {
        attackData.IsInDamageDealingPhase = false;
        //attackSequence.Restart();
        attackSequence.Pause();
    }

    private void WindBackAttack()
    {
        attackData.parentWeaponTransform.DORotate(
                endValue: initialParentRotation.eulerAngles,
                duration: attackData.attackRecoveryTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.InOutBack);
    }

    private void Parried()
    {
        // TODO Maybe this should be part of a sub-state machine for attacking... idk
        // But parry affects animations, so maybe feed into input idk
        StopAttacking();

        Sequence parrySequence = DOTween.Sequence();

        parrySequence.Append(
            attackData.parentWeaponTransform.DORotate(
                endValue: initialParentRotation.eulerAngles,
                duration: attackData.parryRecoilTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.OutQuint));

        parrySequence.AppendInterval(attackData.stunTimeFromParry);

        parrySequence.OnComplete(
            StartAttacking);
    }
}
