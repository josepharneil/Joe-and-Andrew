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

    private void Start()
    {
        initialRotation = parent.rotation.eulerAngles;
        SwingSwordTween();
    }

    public void ResetEnemyAttack()
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
