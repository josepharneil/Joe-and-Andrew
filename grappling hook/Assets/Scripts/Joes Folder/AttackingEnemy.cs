using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AttackingEnemy : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator; 
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Combat")]
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask whatIsDamageable;
    
    [Header("Attacking")]
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    
    [Header("Prototyping")]
    [Tooltip("When this enemy hits, does it knockback the hit entity?")]
    [SerializeField] private bool doesThisEnemyDealKnockback;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackDuration = 1f;
    [Tooltip("When this enemy hits, does it daze the hit entity?")]
    [SerializeField] private bool doesThisEnemyDealDaze;
    [SerializeField] private EntityDaze entityDaze;

    private FacingDirection _facingDirection = FacingDirection.Right; 
    
    private static readonly int PlayerInAttackRange = Animator.StringToHash("playerInAttackRange");
    private static readonly int IsDazed = Animator.StringToHash("isDazed");

    private void Update()
    {
        CheckPlayerInRange();
        
        UpdateFacingDirection();

        animator.SetBool(IsDazed,entityDaze.isDazed);
    }

    private void CheckPlayerInRange()
    {
        float sqDistance = ((Vector2)transform.position - (Vector2)player.transform.position).sqrMagnitude;
        animator.SetBool(PlayerInAttackRange, sqDistance < attackRange * attackRange);
    }

    private void UpdateFacingDirection()
    {
        if (player.transform.position.x < transform.position.x)
        {
            _facingDirection = FacingDirection.Left;
            spriteRenderer.flipX = true;
        }
        else
        {
            _facingDirection = FacingDirection.Right;
            spriteRenderer.flipX = false;
        }
    }

    //todo consider composition of classes for this ?
    /// <summary>
    /// This is called by Animation events
    /// </summary>
    /// <param name="attackIndex"></param>
    private void CheckAttackHitBox(int attackIndex)
    {
        Vector2 overlapCirclePosition;
        if (_facingDirection == FacingDirection.Left)
        {
            overlapCirclePosition = (Vector2)transform.position + new Vector2(-1f, 0 );
        }
        else
        {
            overlapCirclePosition = (Vector2)transform.position + new Vector2(1f, 0 );
        }
        
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            layerMask = whatIsDamageable,
            useLayerMask = true,
        };
        List<Collider2D> detectedObjects = new List<Collider2D>();
        Physics2D.OverlapCircle(overlapCirclePosition, 1f, contactFilter2D, detectedObjects);

        foreach (Collider2D coll in detectedObjects)
        {
            EntityHealth entityHealth = coll.gameObject.GetComponent<EntityHealth>();
            if (entityHealth)
            {
                entityHealth.Damage( attackDamage );
            }
            
            EntityKnockback entityKnockback = coll.gameObject.GetComponent<EntityKnockback>();
            if (entityKnockback && doesThisEnemyDealKnockback)
            {
                entityKnockback.Knockback(entityHealth.transform.position - transform.position, knockbackStrength, knockbackDuration);
            }
            
            if (doesThisEnemyDealDaze)
            {
                EntityDaze entityDaze = coll.gameObject.GetComponent<EntityDaze>();
                if (entityDaze)
                {
                    entityDaze.Daze();
                }
            }
            

            // Instantiate a hit particle here if we want
        }

        // if (sandbagHit)
        // {
        //     cinemachineShake.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
        // }

        // At the end, we're now post damage.
        // inputs.isInPreDamageAttackPhase = false;
    }
}
