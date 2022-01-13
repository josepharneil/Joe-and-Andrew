using System;
using UnityEngine;

public class BaseMeleeWeapon : MonoBehaviour
{
    [Tooltip("This should have a trigger collider.")]
    public MeleeWeaponHitBox weaponHitBox;
    
    [HideInInspector] public bool isInDamageDealingPhase = false;
    protected enum AttackType
    {
        Light,
        Heavy
    }
    protected AttackType CurrentAttackType = AttackType.Light;

    [SerializeField] private int lightAttackDamage = 3;
    [SerializeField] private int heavyAttackDamage = 6;

    public virtual void StartLightAttack(bool isFacingLeft, Action onFinish)
    {
        Debug.LogError("This is not supposed to be called, this is the base class");
    }
    public virtual void StartHeavyAttack(bool isFacingLeft, Action onFinish)
    {
        Debug.LogError("This is not supposed to be called, this is the base class");
    }
    public virtual void SetWeaponActive(bool active)
    {
        Debug.LogError("This is not supposed to be called, this is the base class");
    }
    
    private void OnEnable()
    {
        weaponHitBox.OnWeaponHitEnemy += DealDamageToEnemy;
    }

    private void OnDisable()
    {
        weaponHitBox.OnWeaponHitEnemy -= DealDamageToEnemy;
    }
    
    private void DealDamageToEnemy(EnemyHealth enemyHealth)
    {
        if (!isInDamageDealingPhase)
        {
            return;
        }
        switch (CurrentAttackType)
        {
            case AttackType.Light:
                enemyHealth.Damage(lightAttackDamage);
                break;
            case AttackType.Heavy:
                enemyHealth.Damage(heavyAttackDamage);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}
