using System;
using UnityEngine;
//using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int AttackTriggerID = Animator.StringToHash("attackTrigger");
    public void StartAttack(Action onAttackFinish)
    {
        animator.SetTrigger(AttackTriggerID);

        //animator.GetCurrentAnimatorStateInfo(0).length;
        // Should invoke this at the end of the attack
        onAttackFinish?.Invoke();
    }
}
