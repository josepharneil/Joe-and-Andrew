using System;
using DG.Tweening;
using UnityEngine;

public class SwordWeapon : BaseMeleeWeapon
{
    [Header("Light Attack")]
    [Tooltip("Time for wind back")]
    [SerializeField] private float attackWindUpTime = 0.2f;
    [Tooltip("Duration of swing down attack")]
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float timeBetweenAttackAndRecovery = 0.2f;
    [SerializeField] private float attackRecoveryTime = 1f;

    private Quaternion _initialParentRotation;
    [SerializeField] private Transform parentWeaponTransform;
    //[SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        _initialParentRotation = parentWeaponTransform.rotation;
    }
    
    public override void SetWeaponActive(bool active)
    {
        parentWeaponTransform.gameObject.SetActive(active);
    }

    public override void StartLightAttack(bool isFacingLeft, Action onFinish)
    {
        CurrentAttackType = AttackType.Light;
        
        // Set up attack sequence
        Sequence attackSequence = DOTween.Sequence();

        float windBackDegree = isFacingLeft ? -20 : 20;
        float downSwingDegree = isFacingLeft ? 130 : 360 - 130;
        RotateMode swingDownRotateMode = isFacingLeft ? RotateMode.WorldAxisAdd : RotateMode.Fast;
        
        //attackSequence.AppendCallback(() => { sprite.enabled = true; });

        // Wind back sword
        //attackSequence.AppendInterval(attackWindUpTime);
        attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: new Vector3(0, 0, windBackDegree),
                    duration: attackWindUpTime,
                    mode: RotateMode.FastBeyond360)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    isInDamageDealingPhase = true;
                }));

        // Attack downswing
        attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: new Vector3(0, 0, downSwingDegree),
                    duration: attackDuration,
                    mode: swingDownRotateMode)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    isInDamageDealingPhase = false;
                }));

        // Short delay before bringing back up
        attackSequence.AppendInterval(timeBetweenAttackAndRecovery);

        // Wind back sword to beginning
        attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: _initialParentRotation.eulerAngles,
                    duration: attackRecoveryTime,
                    mode: RotateMode.Fast)
                .SetEase(Ease.InOutBack));

        // On finish
        attackSequence.AppendCallback(onFinish.Invoke);

        //attackSequence.AppendCallback(() => { sprite.enabled = false; });
        
        attackSequence.Play();
    }

    [Header("Heavy Attack")]
    [Tooltip("Time for wind back")]
    [SerializeField] private float heavyAttackWindUpTime = 0.2f;
    [Tooltip("Duration of swing down attack")]
    [SerializeField] private float heavyAttackDuration = 0.8f;
    [SerializeField] private float heavyTimeBetweenAttackAndRecovery = 0.2f;
    [SerializeField] private float heavyAttackRecoveryTime = 1f;

    public override void StartHeavyAttack(bool isFacingLeft, Action onFinish)
    {
        CurrentAttackType = AttackType.Heavy;
        
        // Set up attack sequence
        Sequence attackSequence = DOTween.Sequence();

        float windBackDegree = isFacingLeft ? -20 : 20;
        float downSwingDegree = isFacingLeft ? 130 : 360 - 130;
        RotateMode swingDownRotateMode = isFacingLeft ? RotateMode.WorldAxisAdd : RotateMode.Fast;
        
        //attackSequence.AppendCallback(() => { sprite.enabled = true; });

        // Wind back sword
        //attackSequence.AppendInterval(attackWindUpTime);
        attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: new Vector3(0, 0, windBackDegree),
                    duration: heavyAttackWindUpTime,
                    mode: RotateMode.FastBeyond360)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    isInDamageDealingPhase = true;
                }));

        // Attack downswing
        attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: new Vector3(0, 0, downSwingDegree),
                    duration: heavyAttackDuration,
                    mode: swingDownRotateMode)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    isInDamageDealingPhase = false;
                }));

        // Short delay before bringing back up
        attackSequence.AppendInterval(heavyTimeBetweenAttackAndRecovery);

        // Wind back sword to beginning
        attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: _initialParentRotation.eulerAngles,
                    duration: heavyAttackRecoveryTime,
                    mode: RotateMode.Fast)
                .SetEase(Ease.InOutBack));

        // On finish
        attackSequence.AppendCallback(onFinish.Invoke);

        //attackSequence.AppendCallback(() => { sprite.enabled = false; });
        
        attackSequence.Play();
    }
}