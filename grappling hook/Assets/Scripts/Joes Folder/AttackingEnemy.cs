using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AttackingEnemy : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator; 
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("Combat")]
    [SerializeField] private GameObject player;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private bool doesThisEnemyDealKnockback;
    [SerializeField] private bool doesThisEnemyDealDaze;
    public bool canThisEnemyBeDazed = true;
    public float dazeDuration = 0.8f;
    [SerializeField] private int attackDamage;
    [SerializeField] private float knockbackStrength;

    private FacingDirection _facingDirection = FacingDirection.Right; 
    
    private static readonly int PlayerInAttackRange = Animator.StringToHash("playerInAttackRange");

    private void Update()
    {
        CheckPlayerInRange();

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

    private void CheckPlayerInRange()
    {
        float sqDistance = ((Vector2)transform.position - (Vector2)player.transform.position).sqrMagnitude;
        animator.SetBool(PlayerInAttackRange, sqDistance < attackRange * attackRange);
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

        //bool playerHit = false;
        foreach (Collider2D coll in detectedObjects)
        {
            EntityHealth entityHealth = coll.gameObject.GetComponent<EntityHealth>();
            if (entityHealth)
            {
                entityHealth.Damage( attackDamage );
                
                if (doesThisEnemyDealKnockback)
                {
                    entityHealth.Knockback(entityHealth.transform.position - transform.position, knockbackStrength, 0.5f);
                    //playerHit = true;
                }
                
                if (doesThisEnemyDealDaze)
                {
                    PlayerCombatPrototyping playerCombatPrototyping = coll.gameObject.GetComponent<PlayerCombatPrototyping>();
                    if (playerCombatPrototyping && playerCombatPrototyping.canBeDazedWhenHit)
                    {
                        StartCoroutine(DazePlayer(playerCombatPrototyping));
                    }
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

    private IEnumerator DazePlayer(PlayerCombatPrototyping playerCombatPrototyping)
    {
        playerCombatPrototyping.isDazed = true;
        yield return new WaitForSeconds(playerCombatPrototyping.dazeDuration);
        playerCombatPrototyping.isDazed = false;
    }
}
