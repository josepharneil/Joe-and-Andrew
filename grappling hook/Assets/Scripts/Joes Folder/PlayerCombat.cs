using System;
using System.Collections;
using System.Security.Authentication.ExtendedProtection.Configuration;
using UnityEngine;
//using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UIElements.Experimental;

public class PlayerCombat : MonoBehaviour
{
//    [SerializeField] private Animator animator;

    public void StartAttack(Action onAttackFinish, float attackLength, bool isFacingLeft)
    {
        

        // //var currentState = animator.GetCurrentAnimatorStateInfo(0);//.IsName("Player_Idle");
        // var nextState = animator.GetNextAnimatorStateInfo(0);
        //
        // // while (nextState.fullPathHash == 0)
        // // {
        // //     nextState = animator.GetNextAnimatorStateInfo(0);
        // // }
        //
        // var length = animator.GetCurrentAnimatorStateInfo(0).length;
        // //length = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        // length = animator.GetNextAnimatorStateInfo(0).length;
        
        
        var stepLength = 4f;
        stepLength = isFacingLeft ? -stepLength : stepLength;
        transform.DOMoveX(transform.position.x + stepLength, attackLength).SetEase(Ease.InOutQuint);
        
        StartCoroutine(InvokeAttackFinishAfterSeconds(onAttackFinish, attackLength));
        
        //onAttackFinish?.Invoke();
    }
    
    

    private static IEnumerator InvokeAttackFinishAfterSeconds(Action onAttackFinish, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        onAttackFinish?.Invoke();
    }
}
