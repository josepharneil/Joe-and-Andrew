using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class PlayerAttack : MonoBehaviour, IState
{
    //[SerializeField] private WeaponBase _weaponBase;
    public bool isAttacking = false;

    [Header("Stats")]
    public int attackDamage = 5;

    [Header("Timings")]
    // [SerializeField] private float _parryTimeLimit = 0.2f; // Time frame for player to parry.
    [SerializeField] private float attackWindUpTime = 0.2f;
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float timeBetweenAttackAndRecovery = 0.2f;
    [SerializeField] private float attackRecoveryTime = 1f;

    // Parry
    // [SerializeField] private float _parryRecoilTime = 0.7f;
    // [SerializeField] private float _stunTimeFromParry = 1.5f;

    [Header("Weapon Components")]
    [SerializeField] private Transform parentWeaponTransform;

    public bool isInDamageDealingPhase = false; // Not true if for example the sword is going back up.

    // public bool parried = false;

    // Attack movements
    private Sequence _attackSequence;
    private Quaternion _initialParentRotation;
    
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

    public void OnEnter()
    {
        StartAttacking();
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

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking )
        {
            isAttacking = true;
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
}