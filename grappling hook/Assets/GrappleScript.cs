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
    public Vector3 currentGrapplePosition;
    public Vector3 hitPosition;
    public Vector2 playerHitVector;
    public bool isGrappling;

    //called on open
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    //called each frame
    void Update()
    { 
        HoldableGrapple();
    }

    //Called after Update
    void LateUpdate()
    {

    }

    void StartGrapple(Vector2 playerRbPosition)
    {

        //applies a force with the vector of the distance between the player and the hit
        playerRb.AddForce(playerHitVector * 5, ForceMode2D.Force);

        //new method trying vector2.movetowards - currently working on

        //playerRbPosition.MoveTowards(player.position, hitPosition, 5f);

        //another method trying rigidbody2d.moveposition - its janky af, turn on for good fun
        //playerRb.MovePosition(hitPosition);
        DrawRope();

    }

    bool HitCheck(RaycastHit2D hit, Vector2 playerHitVector)
    {
        //if distance is to too big + is grapplable
        if (hit && playerHitVector.magnitude < grappleLength)
        {
            isGrappling = true;
            return true;
        }
        else
        {
            return false;
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
            if (Input.GetMouseButtonDown(0))
            {
                //gets the current mouse position
                hitPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hit = Physics2D.Raycast(player.position, hitPosition - player.position, grappleLength, whatIsGrappleable);
                playerHitVector = hit.point - new Vector2(player.position.x, player.position.y);
                
            }
            //checks if a valid grapple
            if (HitCheck(hit,playerHitVector))
            {
                
               StartGrapple(playerRb.position);
                
            }
        }
        else
        {
            
             lr.enabled = false;
        }
    }

}
