using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{

    private LineRenderer lr;
    //private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    //public Transform gunTip, camera, player;
    public Transform player;
    public Rigidbody2D playerRb;
    //private float maxDistance = 100f;
    //private SpringJoint joint;
    RaycastHit2D hit;
    public float grappleLength = 15f;
    private Vector3 currentGrapplePosition;
    private Vector2 playerHitVector;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
       
            
        
        HoldableGrapple();
    }

    //Called after Update
    void LateUpdate()
    {
        //don't think that we need any of this anymore
        if( hit )
        {
            
        }
        else
        {
            lr.enabled = false;
        }
    }

    void StartGrapple()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(player.position, worldPosition - player.position, grappleLength, whatIsGrappleable);
        playerHitVector = hit.point - new Vector2(player.position.x, player.position.y);

        //checks if the distance between the player and the surface is too big
        if ( hit && playerHitVector.magnitude < grappleLength)
        {
            //applies a force with the vector of the distance between the player and the hit
            playerRb.AddForce(playerHitVector * 5, ForceMode2D.Force);
            DrawRope();
            
        }
        else
        {
            lr.enabled = false;
        }
    }

    

    void DrawRope()
    {
        
        //If not grappling, don't draw rope
        //if (!joint) return;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //currentGrapplePosition = Vector2.Lerp(player.position, worldPosition, Time.deltaTime * 8f);
        lr.enabled = true;
        lr.SetPosition(0, player.position);
        lr.SetPosition(1, hit.point);
    }

    //grapple which can be cancelled by letting go of the mouse
    void HoldableGrapple()
    {
        if (Input.GetMouseButton(0))
        {
            StartGrapple();
        }
        else
        {
             lr.enabled = false;
        }
    }

    //grapple which is just the point and gooooo 
    void GoGrapple()
    {

    }
    //public bool IsGrappling()
   // {
    //    return joint != null;
    //}

    //public Vector3 GetGrapplePoint()
    //{
    //    return grapplePoint;
    //}
}
