using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackData : MonoBehaviour
{
    [Header("Stats")]
    public int AttackDamage = 5;

    [Header("Timings")]
    public float ParryTimeLimit = 0.2f; // Time frame for player to parry.
    public float attackWindUpTime = 0.2f;
    public float attackDuration = 0.8f;
    public float timeBetweenAttackAndRecovery = 0.2f;
    public float attackRecoveryTime = 1f;

    // Parry
    public float parryRecoilTime = 0.7f;
    public float stunTimeFromParry = 1.5f;

    [Header("Weapon Components")]
    public Transform parentWeaponTransform;

    public bool IsInDamageDealingPhase = false; // Not true if for example the sword is going back up.

    public bool Parried = false;
}
