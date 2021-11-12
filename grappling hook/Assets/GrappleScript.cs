using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    

    [Header("Setup")]
    public LineRenderer lr;
    public Transform player;
    public Rigidbody2D playerRb;
    public LayerMask grappleableLayer;

    [Header("Config")]
    public float grappleForce = 7f;
    public float maxGrappleLength = 15f;

    [Header("Debug Information")]
    public bool isGrappling;
    public Vector2 grappleHitVectorNormalised;
    public float angularVelocity;

    RaycastHit2D maybeGrappleRaycastHit;

    public enum grappleType
    {
        pull,
        fixedLength
    }

    //called each frame
    void Update()
    { 
        HandleGrappleInput();
    }


    // Physics related code (ie AddForce) should be in FixedUpdate.
    // FixedUpdate is called at a fixed rate (the rate of the physics system)
    private void FixedUpdate()
    {
        // If we're grappling, update the hit vector.
        if (IsGrappleHitting(maybeGrappleRaycastHit))
        {
            grappleHitVectorNormalised = (maybeGrappleRaycastHit.point - new Vector2(player.position.x, player.position.y)).normalized;
            UpdateGrapple(playerRb.position);
        }
    }

    // LateUpdate is called right at end, so after all calculations, we draw the rope position
    private void LateUpdate()
    {
        // If we're grappling, update the hit vector.
        if (IsGrappleHitting(maybeGrappleRaycastHit))
        {
            DrawRope();
        }
    }

    void UpdateGrapple(Vector2 playerRbPosition)
    {

        //applies a force with the vector of the distance between the player and the hit
        Vector3 grappleForceVector = grappleHitVectorNormalised * grappleForce;
        playerRb.AddForce(grappleForceVector, ForceMode2D.Force);
        // Note: JA
        // Debug code: useful :D!
        Debug.DrawRay(player.position, grappleHitVectorNormalised);
        // Debug.DrawLine(player.position, maybeGrappleRaycastHit.point);
        // Debug.Log(grappleHitVector);
        // Debug.Log(player.position);


        Vector3 debugForceVector = Vector3.up;
        debugForceVector = Vector3.right;
        //playerRb.AddForce(debugForceVector * grappleForce, ForceMode2D.Force);


        // Method 2??
        // new method trying vector2.movetowards - currently working on
        //playerRbPosition.MoveTowards(player.position, hitPosition, 5f);

        // Method 3??
        //another method trying rigidbody2d.moveposition - its janky af, turn on for good fun
        //playerRb.MovePosition(hitPosition);

        //Method 4 for a fixed grapple;
        //fixedGrapple();
    }

    bool IsGrappleHitting( in RaycastHit2D hit )
    {
        //if distance is to too big + is grapplable
        // note (joe) is the magnitude check necessary? cos raycast should do that already?
        if ( hit )
        {
            isGrappling = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    void StartGrapple()
    {
        // Start grappling
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 grappleCheckDirection = mouseWorldPosition - player.position;
        maybeGrappleRaycastHit = Physics2D.Raycast(player.position, grappleCheckDirection, maxGrappleLength, grappleableLayer);
    }

    void StopGrapple()
    {
        // Stop grappling
        maybeGrappleRaycastHit = new RaycastHit2D();
        isGrappling = false;
        lr.enabled = false;
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
        //currentGrapplePosition = Vector2.Lerp(player.position, worldPosition, Time.deltaTime * 8f);
        lr.enabled = true;
        lr.SetPosition(0, player.position);
        lr.SetPosition(1, maybeGrappleRaycastHit.point);
    }

    //grapple which can be cancelled by letting go of the mouse
    void HandleGrappleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    void fixedGrapple()
    {
        GameObject rod = new GameObject();
        rod.name = "rod";
        rod.AddComponent<Rigidbody2D>();

    }

}
