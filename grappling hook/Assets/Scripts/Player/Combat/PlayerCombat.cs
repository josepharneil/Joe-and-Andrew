using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using JetBrains.Annotations;

public class PlayerCombat : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private PlayerInputs inputs;
    
    [Header("Attack infos")]
    [SerializeField] private Transform sideAttackHitBoxPosition;
    [SerializeField] private Transform aboveAttackHitBoxPosition;
    [SerializeField] private Transform belowAttackHitBoxPosition;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRadius;
    
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

    [Header("Gamepad Vibration")]
    [SerializeField] private GamepadVibrator gamepadVibrator;

    [Header("Knockback")]
    [SerializeField] private EntityKnockback entityKnockback;
    
    [Header("Time Scale On Hit")]
    private float _slowTimeScaleTimer = 0f;
    [SerializeField] private float slowTimeScaleDuration = 0.2f;
    [SerializeField] private float slowTimeScaleAmount = 1 / 20f;

    [Header("Prototyping")]
    public PlayerCombatPrototyping playerCombatPrototyping;

    private void OnEnable()
    {
        upSwipe.enabled = false;
        downSwipe.enabled = false;
        rightSwipe.enabled = false;
        leftSwipe.enabled = false;
    }

    private enum AttackDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Called by Animation Events.
    /// </summary>
    /// <param name="attackIndex"> Index of the attack </param>
    [UsedImplicitly] public void CheckAttackHitBox(int attackIndex)
    {
        FacingDirection facingDirection = inputs.FacingDirection;

        AttackDirection attackDirection = AttackDirection.Right;
        switch (attackIndex)
        {
            // 0, 1 and 2 ... need to look into this
            case 0:
            case 1:
            case 2:
            {
                attackDirection = facingDirection == FacingDirection.Left
                    ? AttackDirection.Left
                    : AttackDirection.Right;
                break;
            }
            case 3:
            {
                attackDirection = AttackDirection.Up;
                break;
            }
            case 4:
            {
                attackDirection = AttackDirection.Down;
                break;
            }
            default:
                Debug.LogError("Unimplemented attack.");
                break;
        }

        Transform attackPosition = sideAttackHitBoxPosition;
        switch (attackDirection)
        {
            case AttackDirection.Up:
                attackPosition = aboveAttackHitBoxPosition;
                StartCoroutine(ShowSwipe(AttackDirection.Up));
                break;
            case AttackDirection.Down:
                attackPosition = belowAttackHitBoxPosition;
                StartCoroutine(ShowSwipe(AttackDirection.Down));
                break;
            case AttackDirection.Left:
                StartCoroutine(ShowSwipe(AttackDirection.Left));
                break;
            case AttackDirection.Right:
                StartCoroutine(ShowSwipe(AttackDirection.Right));
                break;
        }
        
        Vector2 overlapCirclePosition;
        if (facingDirection == FacingDirection.Left)
        {
            var localPosition = attackPosition.localPosition;
            overlapCirclePosition = (Vector2)transform.position + new Vector2(-localPosition.x, localPosition.y );
        }
        else
        {
            overlapCirclePosition = attackPosition.position;
        }
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            layerMask = whatIsDamageable,
            useLayerMask = true,
            useTriggers = true
        };
        List<Collider2D> detectedObjects = new List<Collider2D>();
        Physics2D.OverlapCircle(overlapCirclePosition, attackRadius, contactFilter2D, detectedObjects);
        
        bool enemyHit = false;

        bool firstEnemyHitPositionIsSet = false;
        Vector2 firstEnemyHitPosition = Vector2.zero;
        foreach (Collider2D coll in detectedObjects)
        {
             coll.gameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
             if (!entityHitbox) continue;
             
             int damageDealt = attackDamage;
             if (playerCombatPrototyping.data.doesAttackingParriedDealBonusDamage)
             {
                 damageDealt *= playerCombatPrototyping.data.attackParriedBonusDamageAmount;
             }
                 
             EntityHitData hitData = new EntityHitData
             {
                 DealsDamage = true,
                 DamageToHealth = damageDealt,
                     
                 DealsKnockback = playerCombatPrototyping.data.doesPlayerDealKnockback,
                 KnockbackOrigin = transform.position,
                 KnockbackStrength = playerCombatPrototyping.data.knockbackStrength,
                     
                 DealsDaze = playerCombatPrototyping.data.doesPlayerDealDaze,
             };
             enemyHit = entityHitbox.Hit(hitData);
             
             if (enemyHit && !firstEnemyHitPositionIsSet)
             {
                 firstEnemyHitPosition = entityHitbox.transform.position;
                 firstEnemyHitPositionIsSet = true;
             }
             // Instantiate a hit particle here if we want
        }

        if (enemyHit)
        {
            cinemachineShake.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
            Time.timeScale = slowTimeScaleAmount;
            _slowTimeScaleTimer = slowTimeScaleDuration;
            
            // Todo check this logic
            if (entityKnockback && playerCombatPrototyping.data.doesPlayerGetKnockedBackByOwnAttacks && firstEnemyHitPositionIsSet)
            {
                entityKnockback.StartKnockBack(firstEnemyHitPosition, playerCombatPrototyping.data.selfKnockbackStrength);
            }
            
            // Or here! Instantiate a hit particle here if we want
        }

        // At the end, we're now post damage.
        inputs.isInPreDamageAttackPhase = false;
    }

    private void Update()
    {
        if (_slowTimeScaleTimer > 0f)
        {
            _slowTimeScaleTimer -= Time.unscaledDeltaTime;
            if (_slowTimeScaleTimer < 0f)
            {
                Time.timeScale = 1f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (inputs.FacingDirection == FacingDirection.Left)
        {
            var localPosition = sideAttackHitBoxPosition.localPosition;
            Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y);
            Gizmos.DrawWireSphere(position, attackRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(sideAttackHitBoxPosition.position, attackRadius);
        }

        Gizmos.DrawWireSphere(aboveAttackHitBoxPosition.position, attackRadius);
        Gizmos.DrawWireSphere(belowAttackHitBoxPosition.position, attackRadius);
    }


    private IEnumerator ShowSwipe(AttackDirection index)
    {
        SpriteRenderer swipe = upSwipe;
        if (index != AttackDirection.Up)
        {
            switch (index)
            {
                case AttackDirection.Down:
                    swipe = downSwipe;
                    break;
                case AttackDirection.Left:
                    swipe = leftSwipe;
                    break;
                case AttackDirection.Right:
                    swipe = rightSwipe;
                    break;
                default:
                    Debug.LogError("No such index");
                    break;
            }
        }
        
        swipe.enabled = true;
        
        yield return new WaitForSeconds(swipeShowTime / GetComponent<PlayerInputs>().playerCombatPrototyping.data.attackSpeed);

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

