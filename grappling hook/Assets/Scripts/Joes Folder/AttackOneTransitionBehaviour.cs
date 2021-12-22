using UnityEngine;

public class AttackOneTransitionBehaviour : PlayerStateBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    // }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController(animator).IsAttacking = false;
    }
}