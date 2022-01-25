using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerCombatPrototypeData")]
public class PlayerCombatPrototypeData : ScriptableObject
{
    [Header("Movement and Combat")]
    [Tooltip("Is movement disabled by attacks?")]
    public bool movementDisabledByAttacks;
    [Tooltip("Can you change directions mid attack?")] 
    public bool canChangeDirectionsDuringAttack;
        
    [Header("Attack Speed Style")]
    [Tooltip("Attack speed style")]
    public float attackSpeed = 1f;
        
    [Header("Cancellables")]
    [Tooltip("What can cancel attacks?")]
    public PrototypeCancellables cancellables;
    [Tooltip("In which phases can we cancel an attack?")]
    public PrototypeAttackPhases cancellableAttackPhases;

    [Header("Knockback")]
    [Tooltip("Does player deal knockback?")]
    public bool doesPlayerDealKnockback;
    public float knockbackStrength = 2f;
    public bool doesPlayerGetKnockedBackByOwnAttacks = true;
    public float selfKnockbackStrength = 10f;
    
    [Header("Daze")]
    [Tooltip("Does player deal daze?")]
    public bool doesPlayerDealDaze = true;

    [Header("Parry")]
    [Tooltip("Does attacking a parried enemy deal bonus damage?")]
    public bool doesAttackingParriedDealBonusDamage = false;
    [Tooltip("How much bonus damage?")]
    public int attackParriedBonusDamageAmount = 2;
}

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