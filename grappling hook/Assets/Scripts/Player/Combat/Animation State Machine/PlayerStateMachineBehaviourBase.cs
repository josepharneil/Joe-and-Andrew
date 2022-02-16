using UnityEngine;
using Player;

public class PlayerStateMachineBehaviourBase : StateMachineBehaviour
{
    private PlayerInputs _playerController;
    private PlayerCombat _playerCombat;
    
    protected PlayerInputs GetPlayerController(Animator animator)
    {
        if (_playerController == null)
        {
            _playerController = animator.GetComponent<PlayerInputs>();
        }
        return _playerController;
    }

    protected PlayerCombat GetPlayerCombat(Animator animator)
    {
        if (_playerCombat == null)
        {
            _playerCombat = animator.GetComponent<PlayerCombat>();
        }
        return _playerCombat;
    }
    
    protected void SetSpeedBasedOnPrototypeCustomisation(Animator animator)
    {
        if (GetPlayerController(animator).playerCombatPrototyping.data.attackSpeed == 0f)
        {
            GetPlayerController(animator).playerCombatPrototyping.data.attackSpeed = 1f;
        }

        float baseAttackSpeed = GetPlayerController(animator).playerCombatPrototyping.data.attackSpeed;
        float weaponAttackSpeed = GetPlayerCombat(animator).CurrentMeleeWeapon.WeaponSpeed;
        animator.speed = baseAttackSpeed * weaponAttackSpeed;
    }

    protected static void ResetSpeed(Animator animator)
    {
        animator.speed = 1f;
    }
}
