using DG.Tweening;
using UnityEngine;

public class AttackTwoBehaviour : PlayerStateMachineBehaviourBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController(animator).isAttacking = true;

        SetSpeedBasedOnPrototypeCustomisation(animator);
        
        GetPlayerController(animator).isInPreDamageAttackPhase = true;

        // float moveDistance = 4f;
        // if (GetPlayerController(animator).facingDirection == PlayerControllerCombatScene.FacingDirection.Left)
        // {
        //     moveDistance = -moveDistance;
        // }
        // Transform transform = GetPlayerController(animator).transform;
        // float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // TODO DOTween is not good here. You clip through walls. Use a movement script?
        //transform.DOMoveX(transform.position.x + moveDistance, animationLength).SetEase(Ease.InOutQuint);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetPlayerController(animator).isAttacking = false;
        
        ResetSpeed(animator);
    }
}