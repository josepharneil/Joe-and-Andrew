using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private PlayerInputs inputs;
    
    [Serializable] private struct AttackInfo
    {
        public Transform hitBoxPosition;
        public int damage;
        public float radius;
    }

    [Header("Attack infos")]
    [SerializeField] private AttackInfo attackSide1;
    //[SerializeField] private AttackInfo attackSide2;

    [SerializeField] private AttackInfo attackUp;
    [SerializeField] private AttackInfo attackDown;
    
    [Header("Swipes")]
    [SerializeField] private float swipeShowTime = 1f;
    [SerializeField] private SpriteRenderer upSwipe;
    [SerializeField] private SpriteRenderer downSwipe;
    [SerializeField] private SpriteRenderer leftSwipe;
    [SerializeField] private SpriteRenderer rightSwipe;

    [Header("Shake")]
    [SerializeField] private CinemachineShake cinemachineShake;
    [SerializeField] private float shakeAmplitude = 3f;
    [SerializeField] private float shakeFrequency = 1f;
    [SerializeField] private float shakeDuration = 0.1f;
    
    [Header("Prototyping")]
    public PlayerCombatPrototyping playerCombatPrototyping;

    private static readonly int IsDazed = Animator.StringToHash("isDazed");

    private enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private void OnEnable()
    {
        upSwipe.enabled = false;
        downSwipe.enabled = false;
        rightSwipe.enabled = false;
        leftSwipe.enabled = false;
    }

    /// <summary>
    /// Called by Animation Events.
    /// </summary>
    /// <param name="attackIndex"> Index of the attack </param>
    public void CheckAttackHitBox(int attackIndex)
    {
        ref AttackInfo currentAttackInfo = ref attackSide1;
        FacingDirection attackDirection = inputs.facingDirection;

        // Todo, figure out indexes to make more sense
        switch (attackIndex)
        {
            case 3:
                StartCoroutine(ShowSwipe(SwipeDirection.Up));
                break;
            case 4:
                StartCoroutine(ShowSwipe(SwipeDirection.Down));
                break;
            // 0, 1, 2, left and right, and.. err... something else... nothing...
            default:
            {
                // Swipes
                StartCoroutine(attackDirection == FacingDirection.Left
                    ? ShowSwipe(SwipeDirection.Left)
                    : ShowSwipe(SwipeDirection.Right));

                break;
            }
        }
        // todo up, down
        
        
        switch (attackIndex)
        {
            case 0:
                break;
            case 1:
                //attackInfo = ref attackSide2;
                break;
            case 2:
                // todo didnt mean to skip this number.. need to figure out numbering
                break;
            case 3:
                currentAttackInfo = ref attackUp;
                break;
            case 4:
                currentAttackInfo = ref attackDown;
                break;
            default:
                Debug.LogError("No such attack index");
                break;
        }
        Vector2 overlapCirclePosition;
        if (attackDirection == FacingDirection.Left)
        {
            var localPosition = currentAttackInfo.hitBoxPosition.localPosition;
            overlapCirclePosition = (Vector2)transform.position + new Vector2(-localPosition.x, localPosition.y );
        }
        else
        {
            overlapCirclePosition = currentAttackInfo.hitBoxPosition.position;
        }
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            layerMask = whatIsDamageable,
            useLayerMask = true
        };
        List<Collider2D> detectedObjects = new List<Collider2D>();
        Physics2D.OverlapCircle(overlapCirclePosition, currentAttackInfo.radius, contactFilter2D, detectedObjects);

        bool enemyHit = false;
        foreach (Collider2D coll in detectedObjects)
        {
            EntityHealth entityHealth = coll.gameObject.GetComponent<EntityHealth>();
            if (entityHealth)
            {
                entityHealth.Damage( currentAttackInfo.damage );
                if (playerCombatPrototyping.doesPlayerDealKnockback)
                {
                    entityHealth.Knockback(entityHealth.transform.position - transform.position, playerCombatPrototyping.knockbackStrength, 1.0f);
                    enemyHit = true;
                }

                AttackingEnemy attackingEnemy = coll.gameObject.GetComponent<AttackingEnemy>();
                if (attackingEnemy && attackingEnemy.canThisEnemyBeDazed)
                {
                    StartCoroutine(DazeEnemy(attackingEnemy));
                }
            }
            
            // Instantiate a hit particle here if we want
        }

        if (enemyHit)
        {
            cinemachineShake.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
        }

        // At the end, we're now post damage.
        inputs.isInPreDamageAttackPhase = false;
    }

    private IEnumerator DazeEnemy(AttackingEnemy attackingEnemy)
    {
        attackingEnemy.animator.SetBool(IsDazed, true);
        // Todo this could prob just be controlled by the animator
        yield return new WaitForSeconds(attackingEnemy.dazeDuration);
        attackingEnemy.animator.SetBool(IsDazed, false);
    }

    private void OnDrawGizmos()
    {
        if (inputs.facingDirection == FacingDirection.Left)
        {
            var localPosition = attackSide1.hitBoxPosition.localPosition;
            Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y);
            Gizmos.DrawWireSphere(position, attackSide1.radius);
        }
        else
        {
            Gizmos.DrawWireSphere(attackSide1.hitBoxPosition.position, attackSide1.radius);
        }
        
        // if (controller.facingDirection == PlayerControllerCombatScene.FacingDirection.Left)
        // {
        //     var localPosition = attackSide2.hitBoxPosition.localPosition;
        //     Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y );
        //     Gizmos.DrawWireSphere(position, attackSide2.radius);
        // }
        // else
        // {
        //     Gizmos.DrawWireSphere(attackSide2.hitBoxPosition.position, attackSide2.radius);
        // }
        
        Gizmos.DrawWireSphere(attackUp.hitBoxPosition.position, attackUp.radius);
        Gizmos.DrawWireSphere(attackDown.hitBoxPosition.position, attackDown.radius);
    }


    private IEnumerator ShowSwipe(SwipeDirection index)
    {
        SpriteRenderer swipe = upSwipe;
        if (index != SwipeDirection.Up)
        {
            switch (index)
            {
                case SwipeDirection.Down:
                    swipe = downSwipe;
                    break;
                case SwipeDirection.Left:
                    swipe = leftSwipe;
                    break;
                case SwipeDirection.Right:
                    swipe = rightSwipe;
                    break;
                default:
                    Debug.LogError("No such index");
                    break;
            }
        }
        
        swipe.enabled = true;
        
        yield return new WaitForSeconds(swipeShowTime / GetComponent<PlayerInputs>().playerCombatPrototyping.attackSpeed);

        swipe.enabled = false;
    }

    public void ForceHideSwipes()
    {
        downSwipe.enabled = false;
        upSwipe.enabled = false;
        rightSwipe.enabled = false;
        leftSwipe.enabled = false;
    }
}

