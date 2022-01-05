using DG.Tweening;
using UnityEngine;

public class EntityKnockback : MonoBehaviour
{
    /// <summary>
    /// This probably should take into account the "weight" of the entity, ie. heavier entities get knocked back less.
    /// </summary>
    /// <param name="knockbackDirection"></param>
    /// <param name="knockbackStrength"></param>
    /// <param name="duration"></param>
    public void Knockback(Vector3 knockbackDirection, float knockbackStrength, float duration)
    {
        knockbackDirection.Normalize();
        knockbackDirection.y = 0f;
        knockbackDirection.z = 0f;
        
        transform.DOMove(transform.position + (knockbackDirection*knockbackStrength ), duration).SetEase(Ease.OutCubic);
    }

}
