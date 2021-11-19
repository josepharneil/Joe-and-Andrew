using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_SimpleEnemyBehaviour : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] int moveDuration = 100;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Left;

    [Header("Debug")]
    [SerializeField] private CollideType collideType = CollideType.None;
    [SerializeField] private Vector2 collisionVector;

    private Vector2 position;
    

    private int moveCounter = 0;
    

    enum MoveDirection
    {
        Left  = -1,
        Right = 1,
    }
    enum CollideType
    {
        Player,
        Wall,
        Enemy,
        None
    }
    
    void FixedUpdate()
    {
        Move();
        switch (collideType)
        {
            case CollideType.Player:
                HitPlayer();
                collideType = CollideType.None;
                break;
            case CollideType.Wall:
                ChangeDirection();
                collideType = CollideType.None;
                break;
            case CollideType.Enemy:
                ChangeDirection();
                collideType = CollideType.None;
                break;
            case CollideType.None:
                break;
        }
        

    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collideType = CollideType.Player;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collideType = CollideType.Enemy;
        }
        else if( collision.gameObject.CompareTag("Wall"))
        {
            collideType = CollideType.Wall;
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

    void HitPlayer()
    {
        collisionVector = playerRB.position - rb.position;
        collisionVector = collisionVector.normalized;
        if(Mathf.Abs(collisionVector.x )< 0.5f)
        {
            collisionVector.x = 0.5f * Mathf.Sign(collisionVector.x);
        }
        playerRB.AddForce(collisionVector * forceMultiplier);
    }
   

   
}
