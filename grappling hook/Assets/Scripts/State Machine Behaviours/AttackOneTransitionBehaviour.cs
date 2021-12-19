using System;
using DG.Tweening;
using UnityEngine;

public class AttackOneTransitionBehaviour : PlayerStateBase
{
    // private PlayerControllerCombatScene _playerControllerCombatScene;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    // }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     animator.SetTrigger(AttackTwoID);
    //     playerCombat.ToggleCanReceiveInput();
    //     playerCombat.ClearInputReceived();
    // }
    //
    // private void OnEnable()
    // {
    //     _playerControllerCombatScene = ;
    // }
    //
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController(animator).IsAttacking = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}