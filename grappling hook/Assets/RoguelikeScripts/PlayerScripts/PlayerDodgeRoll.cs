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
    [SerializeField] private float rollSpeed;
    [Header("Debug")]
    private Vector2 playerVelocity;
    private Vector2 holdVelocity;
    private int rollCounter;

    private Color spriteColor;

    public RollState rollState;

    private void Awake()
    {
        rollState = RollState.NotRolling;
        spriteColor = GetComponent<SpriteRenderer>().color;
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

    public void StartRoll(RL_PlayerController.FacingDirection facing)
    {

        Debug.Log("Starting roll");
        playerRB.gravityScale = 0f;
        holdVelocity = playerRB.velocity;
        playerRB.velocity = new Vector2(rollSpeed*(float)facing, holdVelocity.y);
        rollCounter = 0;
        playerCollider.size /= 2;
        rollState = RollState.Rolling;
        GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0.5f);
    }

    public void UpdateRoll(RL_PlayerController.FacingDirection facing)
    {
        if (rollCounter < rollDuration)
        {
            playerRB.velocity = new Vector2(rollSpeed*(float)facing, holdVelocity.y);
            rollCounter++;
        }
        else
        {
            rollState = RollState.End;
        }
    }

    public void EndRoll()
    {
        playerRB.gravityScale = 2.3f;
        Debug.Log("End roll");
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRB.velocity = holdVelocity;
        playerCollider.size *= 2;
        GetComponent<SpriteRenderer>().color = Color.white;
        rollState = RollState.NotRolling;
    }
    
}
