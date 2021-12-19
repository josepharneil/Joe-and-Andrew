using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour
{
    private PlayerControllerCombatScene _playerController;
    protected PlayerControllerCombatScene GetPlayerController(Animator animator)
    {
        if (_playerController == null)
        {
            _playerController = animator.GetComponent<PlayerControllerCombatScene>();
        }
        return _playerController;
    }
}
