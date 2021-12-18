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
    [SerializeField] private float accelerationTime =0.4f;

    //gravity and jumpVelocity are calculated based on the jump heigth and time
    private float gravity;
    private float jumpVelocity;
    Vector3 velocity;

    
    private float lerpCurrent=0f;

    MoveController moveController;

    // Start is called before the first frame update
    void Start()
    {
        gravity = -2 * jumpHeight * Mathf.Pow(timeToJumpHeight, -2);
        jumpVelocity = timeToJumpHeight * Mathf.Abs(gravity);
        moveController = gameObject.GetComponent<MoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (moveController.collisions.above || moveController.collisions.below)
        {
            //stop the acceleration due to gravity while in contact with things
            velocity.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && moveController.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        velocity.y += gravity * Time.deltaTime;


        LerpVelocity(ref velocity.x);
        moveController.Move(velocity * Time.deltaTime);
    }

    void LerpVelocity(ref float velocityX)
    {
        float targetVelocityX = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (velocityX == 0f || Mathf.Sign(velocityX) != Mathf.Sign(Input.GetAxisRaw("Horizontal")))
        {
            lerpCurrent = 0f;
        }
        lerpCurrent = Mathf.Lerp(lerpCurrent, 1f, accelerationRate * Time.deltaTime);
        Debug.Log("Lerp Current: " + lerpCurrent.ToString());
        velocityX = Mathf.Lerp(velocityX, targetVelocityX, accelerationCurve.Evaluate(lerpCurrent));
    }

}
