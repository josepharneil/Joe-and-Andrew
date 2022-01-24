using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;

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
    
    [Header("Time Scale On Hit")]
    private float _slowTimeScaleTimer = 0f;
    [SerializeField] private float slowTimeScaleDuration = 0.2f;
    [SerializeField] private float slowTimeScaleAmount = 1 / 20f;

    [Header("Prototyping")]
    public PlayerCombatPrototyping playerCombatPrototyping;
    
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
        
        Transform attackPosition = sideAttackHitBoxPosition;
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
                attackPosition = aboveAttackHitBoxPosition;
                break;
            case 4:
                attackPosition = belowAttackHitBoxPosition;
                break;
            default:
                Debug.LogError("No such attack index");
                break;
        }
        
        Vector2 overlapCirclePosition;
        if (attackDirection == FacingDirection.Left)
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
        foreach (Collider2D coll in detectedObjects)
        {
             coll.gameObject.TryGetComponent<EntityHitbox>(out EntityHitbox entityHitbox);
             if (entityHitbox)
             {
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
             }
             // Instantiate a hit particle here if we want
        }

        if (enemyHit)
        {
            cinemachineShake.ShakeCamera(shakeAmplitude, shakeFrequency, shakeDuration);
            Time.timeScale = slowTimeScaleAmount;
            _slowTimeScaleTimer = slowTimeScaleDuration;
            
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
        if (inputs.facingDirection == FacingDirection.Left)
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

