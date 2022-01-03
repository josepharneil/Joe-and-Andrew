using UnityEngine;

public class PlayerStateMachineBehaviourBase : StateMachineBehaviour
{
    private PlayerInputs _playerController;
    protected PlayerInputs GetPlayerController(Animator animator)
    {
        if (_playerController == null)
        {
            _playerController = animator.GetComponent<PlayerInputs>();
        }
        return _playerController;
    }

    protected void SetSpeedBasedOnPrototypeCustomisation(Animator animator)
    {
        if (GetPlayerController(animator).playerCombatPrototyping.attackSpeed == 0f)
        {
            GetPlayerController(animator).playerCombatPrototyping.attackSpeed = 1f;
        }
        animator.speed = GetPlayerController(animator).playerCombatPrototyping.attackSpeed;
    }

    protected void ResetSpeed(Animator animator)
    {
        animator.speed = 1f;
    }
}
