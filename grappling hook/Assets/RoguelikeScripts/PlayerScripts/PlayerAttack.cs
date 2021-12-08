using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isAttackInput = false;
    [SerializeField] WeaponBase weapon;

    // Update is called once per frame
    void AttackUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttackInput = true;
        }
    }
}



public class WeaponBase : MonoBehaviour
{
    protected enum WeaponType
    {
        Melee,
        Projectile
    }
    protected WeaponType type;
}

public class SwordWeapon : WeaponBase
{
    public SwordWeapon()
    {
        type = WeaponType.Melee;
    }
}