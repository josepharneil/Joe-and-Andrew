using UnityEngine;
using Player;

public class PlayerStateMachineBehaviourBase : StateMachineBehaviour
{
    private PlayerController _playerController; // TODO this should probably just be a more general "PlayerController"
    
    protected PlayerController GetPlayerController(Animator animator)
    {
        if (_playerController == null)
        {
            _playerController = animator.GetComponent<PlayerController>();
        }
        return _playerController;
    }

    protected void SetSpeedBasedOnPrototypeCustomisation(Animator animator)
    {
        PlayerController playerInputs = GetPlayerController(animator);
        if (playerInputs.PlayerAttacks.PlayerCombatPrototyping.data.attackSpeed == 0f)
        {
            playerInputs.PlayerAttacks.PlayerCombatPrototyping.data.attackSpeed = 1f;
        }

        float baseAttackSpeed = playerInputs.PlayerAttacks.PlayerCombatPrototyping.data.attackSpeed;
        float weaponAttackSpeed = playerInputs.PlayerAttacks.CurrentPlayerEquipment.CurrentMeleeWeapon.WeaponSpeed;
        animator.speed = baseAttackSpeed * weaponAttackSpeed;
    }

    protected static void ResetSpeed(Animator animator)
    {
        animator.speed = 1f;
    }
}
