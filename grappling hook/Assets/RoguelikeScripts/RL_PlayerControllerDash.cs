using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerControllerDash : MonoBehaviour
{
    // Debug sprite renderer
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float dashForce = 40f;
    [SerializeField] private float dashDurationShort = 0.15f;
    [SerializeField] private float dashDurationLong = 0.25f;
    [SerializeField] private float dashChargingThresholdDuration = 0.15f;

    // Timer / duration
    private float dashDurationToUse = 0.1f;
    private float dashTimer = float.MaxValue;

    // @JA TODO BAD BAD BAD CIRCULAR REFERENCE
    [SerializeField] private RLPlayerController playerController;

    private enum DashState
    {
        NotDashingAndUnavailable,
        NotDashingButAble,
        Charging,
        Start,
        Dashing,
        End
    }
    private DashState dashState = DashState.NotDashingAndUnavailable;

    public bool IsNotDashing()
    {
        return dashState == DashState.NotDashingAndUnavailable || dashState == DashState.NotDashingButAble;
    }

    public bool IsDashing()
    {
        return dashState == DashState.Charging || dashState == DashState.Start || dashState == DashState.Dashing || dashState == DashState.End;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleDashInput();
    }

    private void FixedUpdate()
    {
        ApplyDash();
    }

    private void HandleDashInput()
    {
        if (playerController.isGrounded && dashState == DashState.NotDashingAndUnavailable)
        {
            dashState = DashState.NotDashingButAble;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashState == DashState.NotDashingButAble)
        {
            dashState = DashState.Charging;
            dashTimer = 0f;
        }
        if (!Input.GetKey(KeyCode.LeftShift) && dashState == DashState.Charging)
        {
            if (dashTimer < dashChargingThresholdDuration)
            {
                dashDurationToUse = dashDurationShort;
            }
            else
            {
                dashDurationToUse = dashDurationLong;
            }
            dashState = DashState.Start;
        }
    }

    private void ApplyDash()
    {
        if (dashState == DashState.Charging)
        {
            ApplyChargingDash();
        }
        if (dashState == DashState.Start)
        {
            ApplyStartDash();
        }
        if (dashState == DashState.Dashing)
        {
            ApplyUpdateDash();
        }
        if (dashState == DashState.End)
        {
            ApplyEndDash();
        }
    }

    private void ApplyChargingDash()
    {
        playerController.rb.velocity = new Vector2(0, 0);
        playerController.rb.gravityScale = 0.75f;
        dashTimer += Time.deltaTime;
        spriteRenderer.color = Color.green;
    }

    private void ApplyStartDash()
    {
        playerController.rb.velocity = new Vector2(dashForce * (int)playerController.facingDirection, 0);
        playerController.rb.gravityScale = 0f;
        dashTimer = 0f;
        dashState = DashState.Dashing;
        spriteRenderer.color = Color.blue;
    }

    private void ApplyUpdateDash()
    {
        playerController.rb.velocity = new Vector2(playerController.rb.velocity.x, 0);
        dashTimer += Time.deltaTime;
        if (dashTimer >= dashDurationToUse)
        {
            dashState = DashState.End;
        }
    }

    private void ApplyEndDash()
    {
        playerController.rb.gravityScale = 1f;
        playerController.rb.velocity = new Vector2(0, 0);
        dashState = DashState.NotDashingAndUnavailable;
        dashTimer = float.MaxValue;
        spriteRenderer.color = Color.white;
    }


}
