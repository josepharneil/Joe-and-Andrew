using UnityEngine;

/// <summary>
/// If other entities touch this entity, damage them.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EntitySpiky : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private LayerMask spikesWhat;

    [Header("Customisation")]
    [SerializeField] private int spikeDamage = 5;
    [SerializeField] private float spikeKnockbackDistance = 2f;
    [SerializeField] private float spikeKnockbackDuration = 1f;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject colGameObject = col.gameObject;
        
        if ((colGameObject.layer & spikesWhat) != 0) return;
        
        colGameObject.GetComponent<EntityHealth>()?.Damage(spikeDamage);

        colGameObject.GetComponent<EntityKnockback>()?.Knockback(
             colGameObject.transform.position -  transform.position, 
            spikeKnockbackDistance, 
            spikeKnockbackDuration);
    }
}
