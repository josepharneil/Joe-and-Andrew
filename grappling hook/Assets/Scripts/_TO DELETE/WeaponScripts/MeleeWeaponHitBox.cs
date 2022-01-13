using System;
using UnityEngine;

public class MeleeWeaponHitBox : MonoBehaviour
{
    public event Action<EnemyHealth> OnWeaponHitEnemy;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject colliderObject = col.gameObject;
        if (!colliderObject.CompareTag("Enemy"))
        {
            return;
        }
        
        EnemyHealth enemyHealth = colliderObject.GetComponent<EnemyHealth>();
        Debug.Assert( enemyHealth != null, "Expected enemy health script.");
        OnWeaponHitEnemy?.Invoke(enemyHealth);
    }
}
