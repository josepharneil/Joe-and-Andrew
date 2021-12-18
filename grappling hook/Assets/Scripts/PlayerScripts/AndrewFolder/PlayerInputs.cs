using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1

[RequireComponent(typeof( MoveController))]
public class PlayerInputs : MonoBehaviour
{
    [Header("Move Stats")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float timeToJumpHeight = 0.4f;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] [Range(0f,1f)] private float accelerationRate;
    [SerializeField] private AnimationCurve decelerationCurve;
    [SerializeField] [Range(0f, 1f)] private float decelerationRate;
    //gravity and jumpVelocity are calculated based on the jump height and time
    private float gravity;
    private float jumpVelocity;

    private bool _isMoveInput;
    private FacingDirection _facingDirection;
    

    Vector3 velocity;
    Vector2 input;

    [Header("Debug")]
    [SerializeField] private float lerpCurrent=0f;
    [SerializeField] MoveState _moveState;

    MoveController moveController;

    enum FacingDirection
    {
        Left = -1,
        Right = 1
    }

    enum MoveState
    {
        Stopped,
        Accelerating,
        Running,
        Decelerating,
        ChangingDirection
    }

    // Start is called before the first frame update
    void Start()
    {
        gravity = -2 * jumpHeight * Mathf.Pow(timeToJumpHeight, -2);
        jumpVelocity = timeToJumpHeight * Mathf.Abs(gravity);
        moveController = gameObject.GetComponent<MoveController>();
        _moveState = MoveState.Stopped;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        //gravity acceleration, currently does not have a max
        velocity.y += gravity * Time.deltaTime;
        if (moveController.collisions.above || moveController.collisions.below)
        {
            //stop the acceleration due to gravity while in contact with things
            velocity.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && moveController.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        HandelMoveInput();
        SetHorizontalMove();

        moveController.Move(velocity * Time.deltaTime);
    }


    void HandelMoveInput()
    {
        if (input.x != 0)
        {
            _isMoveInput = true;
            _facingDirection = input.x < 0 ? FacingDirection.Left : FacingDirection.Right;
        }
        else
        {
            _isMoveInput = false;
        }
    }

    void SetHorizontalMove()
    {
        if (_isMoveInput)
        {
            switch (_moveState)
            {
                case MoveState.Stopped:
                    //begins the movement, calls sets to accelerating
                    StartMoving();
                    break;
                case MoveState.Accelerating:
                    //accelerates to run speed, sets to moveState to run once at speed
                    Accelerate();
                    break;
                case MoveState.Decelerating:
                    //this will be called if the player starts decelerating and then wants to move again
                    Accelerate();
                    break;
                case MoveState.Running:
                    //continues moving at the current speed
                    Run();
                    break;
                case MoveState.ChangingDirection:
                    //changes the speed to the opposite one
                    break;
            }
        }
        else
        {
            if (_moveState != MoveState.Stopped)
            {
                StopMoving();
                Decelerate();
            }
        }
    }

    void StartMoving()
    {
        lerpCurrent = 0f;
        _moveState = MoveState.Accelerating;
    }

    void StopMoving()
    {
        lerpCurrent = 0f;
        _moveState = MoveState.Decelerating;
    }

    void Accelerate()
    {
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        velocity.x = Mathf.Lerp(velocity.x, moveSpeed * input.x, accelerationCurve.Evaluate(lerpCurrent));
        if (Mathf.Abs(velocity.x) >= Mathf.Abs(input.x * moveSpeed))
        {
            _moveState = MoveState.Running;
        }
    }

    void Run()
    {
        velocity.x = input.x * moveSpeed;
    }

    void Decelerate()
    {
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, decelerationRate * Time.deltaTime);
        velocity.x = Mathf.Lerp(velocity.x, 0f, decelerationCurve.Evaluate(lerpCurrent));
        if (Mathf.Abs(velocity.x) <=0.5f)
        {
            velocity.x = 0f;
            _moveState = MoveState.Stopped;
        }
    }
}
