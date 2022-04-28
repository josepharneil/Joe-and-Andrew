using UnityEngine;

namespace Player
{
    public class AttackBehaviour : PlayerStateMachineBehaviourBase
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).PlayerAttacks.IsAttacking = true;

            SetSpeedBasedOnPrototypeCustomisation(animator);

            GetPlayerController(animator).PlayerAttacks.IsInPreDamageAttackPhase = true;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ResetSpeed(animator);
            
            GetPlayerController(animator).PlayerAttacks.IsAttacking = false;
        }
    }
}
