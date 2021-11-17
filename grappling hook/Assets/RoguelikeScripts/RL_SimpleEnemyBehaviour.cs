using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_SimpleEnemyBehaviour : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D playerRB;

    [SerializeField] private float moveSpeed = 5f;
    private Vector2 position;
    [SerializeField] int moveDuration = 100;
    private MoveDirection moveDirection = MoveDirection.Left;
    private int moveCounter = 0;
    [SerializeField] private bool playerCollide =false;

    enum MoveDirection
    {
        Left  = -1,
        Right = 1,
    }
    void Update()
    {
        Move();

    }

    void FixedUpdate()
    {
        if (playerCollide)
        {
            
            playerRB.AddForce(new Vector2(500f*(int)moveDirection, 100f));
            playerCollide = false;
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollide = true;
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(moveSpeed *(int)moveDirection, 0f);
        moveCounter += 1;
        if(moveCounter == moveDuration)
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        if(moveDirection == MoveDirection.Left)
        {
            moveDirection = MoveDirection.Right;
        }
        else if(moveDirection ==MoveDirection.Right)
        {
            moveDirection = MoveDirection.Left;
        }
        moveCounter = 0;
    }

   

   
}
