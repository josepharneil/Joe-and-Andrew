using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using DG.Tweening;

internal class PlayerStateMachine
{
    public PlayerStateMachine(int numStates)
    {
        OnUpdates = new Func<int>[numStates];
        OnFixedUpdates = new Action[numStates];
        OnEnters = new Action[numStates];
        OnExits = new Action[numStates];
    }

    public int CurrentState;
    
    private Func<int>[] OnUpdates;
    private Action[] OnFixedUpdates;    
    private Action[] OnEnters;    
    private Action[] OnExits;

    
    public void SetState( int state )
    {
        OnExits[CurrentState]?.Invoke();
        
        CurrentState = state;

        OnEnters[CurrentState]?.Invoke();
    }

    public void SetCallbacks( int state, Func<int> onUpdate, Action onFixedUpdate, Action onEnter, Action onExit )
    {
        OnUpdates[state] = onUpdate;
        OnFixedUpdates[state] = onFixedUpdate;
        OnEnters[state] = onEnter;
        OnExits[state] = onExit;
    }

    public void Update()
    {
        int maybeNewState = OnUpdates[CurrentState].Invoke();
        if (CurrentState != maybeNewState)
        {
            SetState(maybeNewState);
        }
    }

    public void FixedUpdate()
    {
        OnFixedUpdates[CurrentState]?.Invoke();
    }
}

public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine _stateMachine;

    #region States

    // List of states
    private const int MovementState = 0;
    private const int AttackState = 1;

    #endregion

    #region Unity Functions
    
    private void Awake()
    {
        SetupStateMachine();

        InitAttackState();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new PlayerStateMachine(2);

        _stateMachine.SetCallbacks(MovementState, MovementUpdate, MovementFixedUpdate, MovementOnEnter, MovementOnExit);
        _stateMachine.SetCallbacks(AttackState, AttackUpdate, AttackFixedUpdate, AttackOnEnter, AttackOnExit);

        _stateMachine.CurrentState = MovementState;

    }

    private void Update() => _stateMachine.Update();
    private void FixedUpdate() => _stateMachine.FixedUpdate();

    #endregion
    
    
    
    
    #region Movement State
    
    [Header("Setup")]
    public Rigidbody2D rb;
    [SerializeField] private Transform rightWallChecker;
    [SerializeField] private Transform leftWallChecker;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float checkGroundRadius = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stats")]
    [SerializeField] private float moveMultiplier = 11f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float wallGrabTimeLimit = 0.25f;


    private enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
    private FacingDirection _facingDirection = FacingDirection.Right; 
    private float _velocityX;
    private bool _isMoveInput = false;
    private bool _isJumpInput = false;
    private bool _isGrounded = false;
    private float _wallGrabTimer = 0f;
    private bool _isWallGrabbing = false;

    
    private void MovementOnEnter(){}
    
    private int MovementUpdate()
    {
        HandleMoveInput();
        HandleJumpInput();
        CheckIfGrounded();
        CheckIfGrabbedToWall();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            return AttackState;
        }

        return MovementState;
    }
    
    private void HandleMoveInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        if (horizontalAxis != 0)
        {
            if (horizontalAxis < 0)
            {
                _facingDirection = FacingDirection.Left;
            }
            else
            {
                _facingDirection = FacingDirection.Right;
            }
            _isMoveInput = true;
            _velocityX = horizontalAxis;
        }
        else
        {
            _isMoveInput = false;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _isJumpInput = true;
        }
    }
    
    private void CheckIfGrabbedToWall()
    {
        if (_isGrounded)
        {
            _wallGrabTimer = 0f;
        }

        Collider2D rightCollider = Physics2D.OverlapCircle(rightWallChecker.position, checkGroundRadius, groundLayer);
        Collider2D leftCollider = Physics2D.OverlapCircle(leftWallChecker.position, checkGroundRadius, groundLayer);
        if (!_isGrounded)
        {
            if (leftCollider)
            {
                _isWallGrabbing = true;
                _wallGrabTimer += Time.deltaTime;
            }
            if (rightCollider)
            {
                _isWallGrabbing = true;
                _wallGrabTimer += Time.deltaTime;
            }
        }

        if (!leftCollider && !rightCollider)
        {
            _isWallGrabbing = false;
        }

        if (_wallGrabTimer < wallGrabTimeLimit)
        {
            _isWallGrabbing = false;
        }
    }

    private void CheckIfGrounded()
    {
        Collider2D overlapCircle = Physics2D.OverlapCircle(groundChecker.position, checkGroundRadius, groundLayer);

        _isGrounded = overlapCircle != null;
    }

    private void MovementFixedUpdate()
    {
        ApplyMove();
        ApplyJump();
        ApplyWallGrab();
    }
    
    private void ApplyMove()
    {
        if (_isMoveInput)
        {
            rb.velocity = new Vector2(_velocityX * moveMultiplier, rb.velocity.y);
        }
    }

    private void ApplyJump()
    {
        if (_isJumpInput)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            _isJumpInput = false;
        }
        // Better jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

    }

    private void ApplyWallGrab()
    {
        if (_isWallGrabbing)
        {
            if (_wallGrabTimer > wallGrabTimeLimit)
            {
                // Slowly slide down
                float velX = rb.velocity.x;
                const float velY = -12;
                rb.velocity = new Vector2(velX, velY);
            }
            else
            {
                // Stay grabbed (default behaviour)
            }
        }
    }

    private void MovementOnExit(){}
    
    #endregion

    #region AttackState
    
    [Header("Stats")]
    public int attackDamage = 5;

    [Header("Timings")]
    public float attackWindUpTime = 0.2f;
    public float attackDuration = 0.8f;
    public float timeBetweenAttackAndRecovery = 0.2f;
    public float attackRecoveryTime = 1f;

    [Header("Weapon Components")]
    public Transform parentWeaponTransform;

    public bool isInDamageDealingPhase = false; // Not true if for example the sword is going back up.
    
    private Quaternion _initialParentRotation; // So we can reset to this.
    private Sequence _attackSequence;

    private void InitAttackState()
    {
        _initialParentRotation = parentWeaponTransform.rotation;

        _attackSequence = DOTween.Sequence();
        _attackSequence.SetAutoKill(false);

        // Set up attack sequence
        _attackSequence.AppendInterval(attackWindUpTime);

        _attackSequence.AppendCallback(() =>
        {
            isInDamageDealingPhase = true;
        });

        _attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: new Vector3(0, 0, 130),
                    duration: attackDuration,
                    mode: RotateMode.WorldAxisAdd)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    isInDamageDealingPhase = false;
                }));

        _attackSequence.AppendInterval(timeBetweenAttackAndRecovery);

        _attackSequence.Append(
            parentWeaponTransform.DORotate(
                    endValue: _initialParentRotation.eulerAngles,
                    duration: attackRecoveryTime,
                    mode: RotateMode.Fast)
                .SetEase(Ease.InOutBack));


        // For now, just infinite loops.
        //_attackSequence.SetLoops(-1, LoopType.Restart);
        
        _attackSequence.AppendCallback(() =>
        {
            _stateMachine.SetState( MovementState );
        });

        _attackSequence.Pause();
    }
    
    private void AttackOnEnter()
    {
        StartAttacking();
    }

    private int AttackUpdate()
    {
        return AttackState;
    }

    private void AttackFixedUpdate(){}
    
    private void StartAttacking()
    {
        parentWeaponTransform.rotation = _initialParentRotation;
        isInDamageDealingPhase = true;
        _attackSequence.Restart();
        _attackSequence.Play();
    }

    private void StopAttacking()
    {
        isInDamageDealingPhase = false;
        //attackSequence.Restart();
        _attackSequence.Pause();
    }

    private void WindBackAttack()
    {
        parentWeaponTransform.DORotate(
                endValue: _initialParentRotation.eulerAngles,
                duration: attackRecoveryTime,
                mode: RotateMode.Fast)
            .SetEase(Ease.InOutBack);
    }

    private void AttackOnExit()
    {
        StopAttacking();
        WindBackAttack();
    }

    #endregion
}

