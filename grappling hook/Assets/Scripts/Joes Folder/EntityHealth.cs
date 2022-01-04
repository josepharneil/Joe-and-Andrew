using System;
using DG.Tweening;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage( int damage )
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        print("Damage! " + maxHealth);
    }

    public void Heal( int heal )
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        print("Heal! " + maxHealth);
    }

    // todo this should prob not be in health.
    public void Knockback(Vector3 hitDirection, float knockbackStrength, float duration)
    {
        hitDirection.Normalize();
        hitDirection.y = 0f;
        hitDirection.z = 0f;
        
        transform.DOMove(transform.position + (hitDirection*knockbackStrength ), duration).SetEase(Ease.OutCubic);
    }
}
