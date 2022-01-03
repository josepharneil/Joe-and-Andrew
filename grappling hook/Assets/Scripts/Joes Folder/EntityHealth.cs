using DG.Tweening;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private int health;
    public void Damage( int damage )
    {
        health -= damage;
        print("Hit! " + health);
    }

    public void Heal( int heal )
    {
        health += heal;
        print("Hit! " + health);
    }

    public void Knockback(Vector3 hitDirection, float knockbackStrength, float duration)
    {
        hitDirection.Normalize();
        hitDirection.y = 0f;
        hitDirection.z = 0f;
        
        transform.DOMove(transform.position + (hitDirection*knockbackStrength ), duration).SetEase(Ease.OutCubic);
    }
}
