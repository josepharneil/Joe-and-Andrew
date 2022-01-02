using System;
using DG.Tweening;
using UnityEngine;

public class AttackingEnemy : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private float attackRange;

    [SerializeField] private Animator animator; 
    
    [SerializeField] private int health;
    private static readonly int PlayerInAttackRange = Animator.StringToHash("playerInAttackRange");

    // todo consider composition of classes
    // eg. a "damageable" has this function 
    public void DamageThisEnemy( int damage, Vector3 hitDirection, float knockbackStrength )
    {
        health -= damage;
        print("Hit! " + health);

        hitDirection.Normalize();
        hitDirection.y = 0f;
        hitDirection.z = 0f;
        transform.DOMove(transform.position + (hitDirection*knockbackStrength ), 1f).SetEase(Ease.OutCubic);
    }

    private void OnGUI()
    {
        GUILayout.TextArea("Text here!" + health.ToString());
    }

    private void Update()
    {
        CheckPlayerInRange();
    }

    private void CheckPlayerInRange()
    {
        float sqDistance = ((Vector2)transform.position - (Vector2)player.transform.position).sqrMagnitude;
        animator.SetBool(PlayerInAttackRange, sqDistance < attackRange * attackRange);
    }

    //todo this is called by an event
    //todo consider composition of classes for this ?
    private void CheckAttackHitBox(int attackIndex)
    {
        
    }
}
