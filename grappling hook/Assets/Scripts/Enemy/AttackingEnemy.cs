using System.Collections.Generic;
using UnityEngine;
using Entity;

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
    [Tooltip("When this enemy hits, does it daze the hit entity?")]
    [SerializeField] private bool doesThisEnemyDealDaze;
    [SerializeField] private EntityDaze entityDaze;
    [SerializeField] private EntityParryable parryable;

    private FacingDirection _facingDirection = FacingDirection.Right; 
    
    private static readonly int PlayerInAttackRange = Animator.StringToHash("playerInAttackRange");
    private static readonly int IsDazed = Animator.StringToHash("isDazed");
    private static readonly int IsParried = Animator.StringToHash("isParried");

    private void Update()
    {
        CheckPlayerInRange();
        
        UpdateFacingDirection();

        animator.SetBool(IsDazed, entityDaze && entityDaze.isDazed );
        animator.SetBool(IsParried, parryable && parryable.hasBeenParried);
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
        Vector2 overlapCirclePosition = (Vector2)transform.position;
        if (_facingDirection == FacingDirection.Left)
        {
            overlapCirclePosition += new Vector2(-1f, 0 );
        }
        else
        {
            overlapCirclePosition += new Vector2(1f, 0 );
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
            int actualDamageDealt = attackDamage;
            
            EntityBlock entityBlock = coll.gameObject.GetComponent<EntityBlock>();
            if (entityBlock && entityBlock.isBlocking)
            {
                actualDamageDealt /= entityBlock.blockDamageReductionFactor;
                entityBlock.BreakBlock();
            }
            
            EntityHealth entityHealth = coll.gameObject.GetComponent<EntityHealth>();
            if (entityHealth)
            {
                entityHealth.Damage( actualDamageDealt );
            }
            
            EntityKnockback entityKnockback = coll.gameObject.GetComponent<EntityKnockback>();
            if (entityKnockback && doesThisEnemyDealKnockback)
            {
                entityKnockback.Knockback(entityHealth.transform.position - transform.position, knockbackStrength);
            }
            
            if (doesThisEnemyDealDaze)
            {
                coll.gameObject.GetComponent<EntityDaze>()?.Daze();
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
