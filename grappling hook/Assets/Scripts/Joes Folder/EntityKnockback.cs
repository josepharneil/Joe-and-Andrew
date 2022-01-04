using DG.Tweening;
using UnityEngine;

public class EntityKnockback : MonoBehaviour
{
    /// <summary>
    /// This probably should take into account the "weight" of the entity, ie. heavier entities get knocked back less.
    /// </summary>
    /// <param name="hitDirection"></param>
    /// <param name="knockbackStrength"></param>
    /// <param name="duration"></param>
    public void Knockback(Vector3 hitDirection, float knockbackStrength, float duration)
    {
        hitDirection.Normalize();
        hitDirection.y = 0f;
        hitDirection.z = 0f;
        
        transform.DOMove(transform.position + (hitDirection*knockbackStrength ), duration).SetEase(Ease.OutCubic);
    }

}
