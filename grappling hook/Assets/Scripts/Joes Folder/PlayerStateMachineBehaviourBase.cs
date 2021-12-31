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
}
