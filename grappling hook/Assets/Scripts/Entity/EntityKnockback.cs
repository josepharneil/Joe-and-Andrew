using DG.Tweening;
using Entity;
using System;
using UnityEngine;

public class EntityKnockback : MonoBehaviour
{
    private Vector2 _knockbackDirection;
    private float _knockbackStrength;
    private bool _isBeingKnockedBack = false;

    [SerializeField] float knockBackDecrementAmount;
    [SerializeField] private MovementController movementController;
    /// <summary>
    /// This probably should take into account the "weight" of the entity, ie. heavier entities get knocked back less.
    /// </summary>
    /// <param name="knockbackDirection"></param>
    /// <param name="knockbackStrength"></param>
    /// 
    public void StartKnockBack(Vector2 knockbackDirection,float knockbackStrength)
    {
        _knockbackDirection = knockbackDirection;
        _knockbackDirection.Normalize();
        _knockbackStrength = knockbackStrength;
        _isBeingKnockedBack = true;
    }
    public bool UpdateKnockback()
    {
        if (_knockbackStrength <= 0)
        {
            _isBeingKnockedBack = false;
            return _isBeingKnockedBack;
        }
        movementController.MoveAtSpeed(_knockbackDirection * _knockbackStrength);
        _knockbackStrength -= knockBackDecrementAmount*Time.deltaTime;
        return _isBeingKnockedBack;
    }
    public void Knockback(Vector2 knockbackDirection, float knockbackStrength)
    {
        knockbackDirection.Normalize();
        knockbackDirection.y = 0f;
        // transform.DOMove(transform.position + (knockbackDirection*knockbackStrength ), duration).SetEase(Ease.OutCubic);
        movementController.MoveAtSpeed(knockbackDirection * knockbackStrength);
    }

}
