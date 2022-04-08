using UnityEngine;
using Player;

public class PlayerStateMachineBehaviourBase : StateMachineBehaviour
{
    private PlayerInputs _playerInputs; // TODO this should probably just be a more general "PlayerController"
    
    protected PlayerInputs GetPlayerController(Animator animator)
    {
        if (_playerInputs == null)
        {
            _playerInputs = animator.GetComponent<PlayerInputs>();
        }
        return _playerInputs;
    }

    protected void SetSpeedBasedOnPrototypeCustomisation(Animator animator)
    {
        PlayerInputs playerInputs = GetPlayerController(animator);
        if (playerInputs.playerCombatPrototyping.data.attackSpeed == 0f)
        {
            playerInputs.playerCombatPrototyping.data.attackSpeed = 1f;
        }

        float baseAttackSpeed = playerInputs.playerCombatPrototyping.data.attackSpeed;
        float weaponAttackSpeed = playerInputs.CurrentPlayerEquipment.CurrentMeleeWeapon.WeaponSpeed;
        animator.speed = baseAttackSpeed * weaponAttackSpeed;
    }

    protected static void ResetSpeed(Animator animator)
    {
        animator.speed = 1f;
    }
}
