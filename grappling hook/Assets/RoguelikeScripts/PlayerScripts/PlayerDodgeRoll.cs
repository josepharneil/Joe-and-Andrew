using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerDodgeRoll : MonoBehaviour
{
    [Header("components")]
    [SerializeField] RL_PlayerStats playerStats;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] Transform playerTransform;

    [Header("Config")]
    [SerializeField] private float rollDuration = 30;

    [Header("Debug")]
    private Vector2 playerVelocity;
    private Vector2 holdVelocity;

    public RollState rollState;

    private void Awake()
    {
        rollState = RollState.NotRolling;
    }

    public enum RollState
    {
        NotRolling,
        Start,
        Rolling,
        End
    }

    public void DoRoll(bool isGrounded,RL_PlayerController.FacingDirection facingDirection)
    {
        playerRB.constraints = RigidbodyConstraints2D.None;
        Debug.Log("Doing roll");
        if (isGrounded && rollState == RollState.NotRolling)
        {
            rollState = RollState.Start;
            StartRoll(facingDirection);
        }
    }

    public void StartRoll(RL_PlayerController.FacingDirection facingDirection)
    {
        Debug.Log("starting roll");
        //TODO Make invulnerable
        holdVelocity = playerRB.velocity;
        playerRB.velocity = playerRB.velocity * 1.5f;
        rollState = RollState.Rolling;
        playerTransform.DORotate(new Vector3(0f,0f,-360f*(float)facingDirection), rollDuration, RotateMode.FastBeyond360).SetEase(Ease.Unset).OnComplete(()=>
        {
            EndRoll();
        });

    }

    public void EndRoll()
    {
        Debug.Log("End roll");
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRB.velocity = holdVelocity;
        rollState = RollState.NotRolling;
    }
    
}
