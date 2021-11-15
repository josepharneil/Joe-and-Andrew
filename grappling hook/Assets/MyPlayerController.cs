using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    // https://craftgames.co/unity-2d-platformer-movement/

    [Header("Config")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D collider;
    [SerializeField] private float moveScale = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private bool isSliding;
    [SerializeField] private float slideScaler = 250f;
    [SerializeField] private float slideDuration = 10f;

    //slide scaler is used to tweak how fast the slide rotates


    private Vector2 velocity;
    private bool isGrounded = false;
    private Vector2 slideStartSpeed;
    private float slideSign;

    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public GrappleScript grappleScript;


    // Update is called once per frame
    void Update()
    {
        if(!EffectorManager.Instance.CurrentEffectsDisablePlayerInput()||!isSliding)
        {
            Move();
            Jump();
            BetterJump();
            CheckIfGrounded();
            Slide();
        }
        if (isSliding)
        {
            rb.velocity =new Vector2( slideStartSpeed.x,rb.velocity.y);
        }
    }

    private void Move()
    {
        if (!grappleScript.isGrappling || !isSliding)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            if (horizontalAxis != 0 )
            {
                velocity.x = horizontalAxis;
                rb.velocity = new Vector2(velocity.x * moveScale, rb.velocity.y);
            }
        }
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void CheckIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    void Slide()
    {
        if (Input.GetKeyDown(KeyCode.S) && isGrounded == true && rb.velocity.x != 0)
        {
            isSliding = true;
            slideStartSpeed = rb.velocity;
            slideSign = Mathf.Sign(slideStartSpeed.x);
            StartCoroutine("StartSlide");
        }
    }

    IEnumerator StartSlide()
    {
        for (float i = 1;i <=slideDuration; i++)
        {
            //rotates
            collider.transform.Rotate(0f,0f,90f/slideDuration *slideSign);
            yield return new WaitForSeconds(slideDuration/slideScaler);
        }
        yield return new WaitForSeconds(slideDuration/20f);
        StartCoroutine("EndSlide");
    }

    IEnumerator EndSlide()
    {
        for (float i = slideDuration; i > 0; i--)
        {
            //rotates the collider back up 90 degrees based on the slide time
            collider.transform.Rotate(0f, 0f, -90f / slideDuration * slideSign);
            yield return new WaitForSeconds(slideDuration/slideScaler);
        }
        isSliding = false;
    }

    
}
