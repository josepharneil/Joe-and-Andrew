using UnityEngine;

public class WeaponAttackTransitionBehaviour : PlayerStateMachineBehaviourBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Relies on AnimationWeaponStateName having the exactly correct string name that the state does.
        int weaponAnimatorStateHash = GetPlayerController(animator).PlayerAttacks.CurrentPlayerEquipment
            .CurrentMeleeWeapon.AnimationWeaponStateName.ToAnimatorStateHash();
        animator.Play(weaponAnimatorStateHash);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

}
