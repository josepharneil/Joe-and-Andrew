using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatPrototyping : MonoBehaviour
{
    [Header("Prototype Customisation")]
    [Tooltip("Is movement disabled by attacks?")]
    public bool movementDisabledByAttacks;
        
    [Tooltip("Can you change directions mid attack?")] 
    public bool canChangeDirectionsDuringAttack;
        
    [Tooltip("Attack speed style")]
    public float attackSpeed;
        
    [Tooltip("What can cancel attacks?")]
    public PrototypeCancellables cancellables;

    [Tooltip("In which phases can we cancel an attack?")]
    public PrototypeAttackPhases cancellableAttackPhases;

    [Tooltip("Does player deal knockback?")]
    public bool doesPlayerDealKnockback;
    public float knockbackStrength;

    [Tooltip("Quick-set an attack style")]
    [SerializeField] public PrototypeAttackStyles prototypeAttackStyle;
    private PrototypeAttackStyles _prevPrototypeAttackStyle = PrototypeAttackStyles.None;

    [Tooltip("When player is hit, is control taken away?")]
    public bool canBeDazedWhenHit = false;
    [HideInInspector] public bool isDazed = false;
    [Tooltip("When player is hit, how long are they dazed for?")]
    public float dazeDuration = 0.5f;

    private void OnValidate()
    {
        if (prototypeAttackStyle != _prevPrototypeAttackStyle)
        {
            _prevPrototypeAttackStyle = prototypeAttackStyle;
            switch (prototypeAttackStyle)
            {
                case PrototypeAttackStyles.None:
                    movementDisabledByAttacks = false;
                    canChangeDirectionsDuringAttack = false;
                    attackSpeed = 1.0f;
                    cancellables = PrototypeCancellables.None;
                    cancellableAttackPhases = PrototypeAttackPhases.None;
                    doesPlayerDealKnockback = false;
                    knockbackStrength = 1f;
                    break;
                case PrototypeAttackStyles.HollowKnight:
                    movementDisabledByAttacks = false;
                    canChangeDirectionsDuringAttack = false;
                    attackSpeed = 2.0f;
                    cancellables = PrototypeCancellables.None;
                    cancellableAttackPhases = PrototypeAttackPhases.None;
                    doesPlayerDealKnockback = true;
                    knockbackStrength = 2.5f;
                    break;
                case PrototypeAttackStyles.EasyCancel:
                    movementDisabledByAttacks = true;
                    canChangeDirectionsDuringAttack = false;
                    attackSpeed = 1.0f;
                    cancellables = PrototypeCancellables.Roll & PrototypeCancellables.Jump;
                    cancellableAttackPhases = PrototypeAttackPhases.PreDamage;
                    doesPlayerDealKnockback = true;
                    knockbackStrength = 1f;
                    break;
                case PrototypeAttackStyles.DarkSouls:
                    movementDisabledByAttacks = true;
                    canChangeDirectionsDuringAttack = false;
                    attackSpeed = 0.5f;
                    cancellables = PrototypeCancellables.Roll;
                    cancellableAttackPhases = PrototypeAttackPhases.PostDamage;
                    doesPlayerDealKnockback = true;
                    knockbackStrength = 1f;
                    break;
                default:
                    Debug.LogError("Out of range");
                    break;
            }
        }
    }
}

// TODO possibly do different kinds of animations
// Currently the animation is linear frame by frame,
// but for example we could stutter actual hitting later
// Attack speed type
// public enum PrototypeAttackSpeedStyle
// {
//     Fast,
//     Middling,
//     Slow,
// }

// What can cancel attacks?
[Flags] public enum PrototypeCancellables
{
    None = 0,
    Movement = 1 << 0,
    Roll = 1 << 1,
    Jump = 1 << 2,
}

// Phases of an attack.
[Flags] public enum PrototypeAttackPhases
{
    None = 0,
    PreDamage = 1 << 0,
    //DamageFrame - Not allowed to be cancelled as its a single frame for now.
    PostDamage = 2 << 0
}
    
public enum PrototypeAttackStyles
{
    None,
    HollowKnight,
    EasyCancel,
    DarkSouls,
}
