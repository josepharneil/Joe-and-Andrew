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

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lr.enabled = false;
        }
    }

    //Called after Update
    void LateUpdate()
    {
        //DrawRope();
        if( hit )
        {
            DrawRope();
        }
        else
        {
            lr.enabled = false;
        }
    }

    void StartGrapple()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast( player.position, worldPosition - player.position, 8, whatIsGrappleable);
        if ( hit )
        {
            lr.enabled = true;
        }
        else
        {
           // lr.enabled = false;
        }
    }


    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        //if (!joint) return;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //currentGrapplePosition = Vector2.Lerp(player.position, worldPosition, Time.deltaTime * 8f);

        lr.SetPosition(0, player.position);
        lr.SetPosition(1, hit.point);

        playerRb.AddForce(  (hit.point - new Vector2(player.position.x, player.position.y)) * 10);
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
