using DG.Tweening;
using Entity;
using UnityEngine;

public class EntityKnockback : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    /// <summary>
    /// This probably should take into account the "weight" of the entity, ie. heavier entities get knocked back less.
    /// </summary>
    /// <param name="knockbackDirection"></param>
    /// <param name="knockbackStrength"></param>
    public void Knockback(Vector2 knockbackDirection, float knockbackStrength)
    {
        knockbackDirection.Normalize();
        knockbackDirection.y = 0f;
        // transform.DOMove(transform.position + (knockbackDirection*knockbackStrength ), duration).SetEase(Ease.OutCubic);
        movementController.MoveAtSpeed(knockbackDirection * knockbackStrength);
    }

}
