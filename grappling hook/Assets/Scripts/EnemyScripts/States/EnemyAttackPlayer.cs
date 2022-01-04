using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAttackPlayer: MonoBehaviour, IState
{
    [Header("Stats")]
    public int attackDamage = 5;

    [Header("Timings")]
    public float parryTimeLimit = 0.2f; // Time frame for player to parry.
    [SerializeField] private float attackWindUpTime = 0.2f;
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float timeBetweenAttackAndRecovery = 0.2f;
    [SerializeField] private float attackRecoveryTime = 1f;

    // Parry
    [SerializeField] private float parryRecoilTime = 0.7f;
    [SerializeField] private float stunTimeFromParry = 1.5f;

    [Header("Weapon Components")]
    [SerializeField] private Transform parentWeaponTransform;

    public bool isInDamageDealingPhase = false; // Not true if for example the sword is going back up.

    public bool parried = false;


    public void OnEnter()
    {
        StartAttacking();
    }

    public void Tick()
    {
        if (!parried) return;
        Parried();
        parried = false;
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

    private Quaternion _initialParentRotation; // So we can reset to this.
    private Sequence _attackSequence;

    private void Awake()
    {
        _initialParentRotation = parentWeaponTransform.rotation;

        _attackSequence = DOTween.Sequence();

        // Set up attack sequence
        _attackSequence.AppendInterval(attackWindUpTime);

        _attackSequence.AppendCallback(() =>
        {
            isInDamageDealingPhase = true;
        });

        _attackSequence.Append(
            parentWeaponTransform.DORotate(
               endValue: new Vector3(0, 0, 130),
               duration: attackDuration,
               mode: RotateMode.WorldAxisAdd)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        isInDamageDealingPhase = false;
                    }));

        _attackSequence.AppendInterval(timeBetweenAttackAndRecovery);

        _attackSequence.Append(
            parentWeaponTransform.DORotate(
                endValue: _initialParentRotation.eulerAngles,
                duration: attackRecoveryTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.InOutBack));


        // For now, just infinite loops.
        _attackSequence.SetLoops(-1, LoopType.Restart);

        _attackSequence.Pause();
    }

    private void StartAttacking()
    {
        parentWeaponTransform.rotation = _initialParentRotation;
        isInDamageDealingPhase = true;
        _attackSequence.Restart();
        _attackSequence.Play();
    }

    private void StopAttacking()
    {
        isInDamageDealingPhase = false;
        //attackSequence.Restart();
        _attackSequence.Pause();
    }

    private void WindBackAttack()
    {
        parentWeaponTransform.DORotate(
                endValue: _initialParentRotation.eulerAngles,
                duration: attackRecoveryTime,
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
            parentWeaponTransform.DORotate(
                endValue: _initialParentRotation.eulerAngles,
                duration: parryRecoilTime,
                mode: RotateMode.Fast)
                    .SetEase(Ease.OutQuint));

        parrySequence.AppendInterval(stunTimeFromParry);

        parrySequence.OnComplete(
            StartAttacking);
    }
}
