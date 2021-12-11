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
    [SerializeField] CapsuleCollider2D playerCollider;

    [Header("Config")]
    [SerializeField] private int rollDuration = 30;

    [Header("Debug")]
    private Vector2 playerVelocity;
    private Vector2 holdVelocity;
    private int rollCounter;

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

    public void DoRoll(bool isGrounded)
    {

        Debug.Log("Roll Active");
        if (isGrounded && rollState == RollState.NotRolling)
        {
            rollState = RollState.Start;
        }
    }

    public void StartRoll()
    {

        Debug.Log("Starting roll");
        holdVelocity = playerRB.velocity;
        playerRB.velocity = new Vector2(holdVelocity.x * 2f, holdVelocity.y);
        rollCounter = 0;
        playerCollider.transform.localScale /= 2;
        rollState = RollState.Rolling;
    }

    public void UpdateRoll()
    {
        if (rollCounter < rollDuration)
        {
            playerRB.velocity = new Vector2(holdVelocity.x * 2f, holdVelocity.y);
            rollCounter++;
        }
        else
        {
            rollState = RollState.End;
        }
    }

    public void EndRoll()
    {
        //playerRB.gravityScale = 2.3f;
        Debug.Log("End roll");
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRB.velocity = holdVelocity;
        playerCollider.transform.localScale *= 2;
        rollState = RollState.NotRolling;
    }
    
}
