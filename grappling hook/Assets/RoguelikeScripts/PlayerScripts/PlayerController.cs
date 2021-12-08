using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerAttack attack;

    private enum PlayerState
    {
        Movement, // In movement mode
        Attacking, // In attack mode, can't move
        Dead
    }
    private PlayerState state;

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case PlayerState.Movement:
                {
                    movement.MovementUpdate();
                    break;
                }
            case PlayerState.Attacking:
                break;
            case PlayerState.Dead:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Movement:
                {
                    movement.MovementFixedUpdate();
                    break;
                }
            case PlayerState.Attacking:
                break;
            case PlayerState.Dead:
                break;
        }
    }
}
