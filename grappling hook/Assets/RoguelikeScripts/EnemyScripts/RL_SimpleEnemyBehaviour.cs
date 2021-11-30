using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_SimpleEnemyBehaviour : MonoBehaviour
{
    [Header("Config")]
    // TODO Not sure if using a rigid body is a great idea? Might be fine though.
    // Might have to only enable rigidbodies when close to the player.  
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int moveDuration = 100;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Left;
    // TODO Probably good to have scriptable object for enemy stats?
    [SerializeField] private int damage = 5;
    [SerializeField] private int startingHealth =5;

    
    public int currentHealth;

    [Header("Debug")]
    [SerializeField] private CollideType currentCollisionType = CollideType.None;
    [SerializeField] private Vector2 collisionVector;


    //AK: Will probably have to adapt this at some point for other enemy types, currently applied to shooter enemy
    // a bit hacky tho as the shooter doesn't move or have a rigidbody atm
    //doesn't break anything as far as I can tell for now though

    void Start()
    {
        currentHealth = startingHealth;
    }

    private int moveCounter = 0;
    private enum MoveDirection
    {
        Left  = -1,
        Right = 1,
    }
    private enum CollideType
    {
        None,
        Player,
        Wall,
        Enemy,
    }

    // This is different from "Dead". We might want to do something special when
    // dead, e.g release a poisonous gas. Destroy means we're done with it. Delete!
    public enum EnemyState
    {
        Alive,
        Dead,
        Destroy, 
    }
    private EnemyState currentEnemyState = EnemyState.Alive;
    public EnemyState CurrentEnemyState
    {
        get => currentEnemyState;
        set => currentEnemyState = value;
    }

    // NOTE: Collision is like an expensive raycast, could raycast? Might be complicated.
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            currentCollisionType = CollideType.Player;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            currentCollisionType = CollideType.Enemy;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            currentCollisionType = CollideType.Wall;
        }
    }

    // This is effectively this object's fixed update, except its called by
    // the EnemyManager.
    public void EnemyUpdate()
    {
        switch (currentEnemyState)
        {
            case EnemyState.Alive:
                {
                    UpdateMove();
                    UpdateCollision();
                    UpdateHealth();
                    break;
                }
            case EnemyState.Dead:
                {
                    MakeDead();
                    break;
                }
            case EnemyState.Destroy:
                {
                    break;
                }
        }
        if(currentEnemyState == EnemyState.Alive)
        {
        }
    }

    private void UpdateCollision()
    {
        switch (currentCollisionType)
        {
            case CollideType.Player:
                HitPlayer();
                currentCollisionType = CollideType.None;
                break;
            case CollideType.Wall:
                ChangeDirection();
                currentCollisionType = CollideType.None;
                break;
            case CollideType.Enemy:
                ChangeDirection();
                currentCollisionType = CollideType.None;
                break;
            case CollideType.None:
                break;
            
        };
    }

    // Update the movement of the enemy.
    private void UpdateMove()
    {
        if(moveSpeed != 0)
        {
            rb.velocity = new Vector2(moveSpeed *(int)moveDirection, 0f);
            moveCounter += 1;
            if(moveCounter == moveDuration)
            {
                ChangeDirection();
            }
        }
    }

    private void ChangeDirection()
    {
        if(moveDirection == MoveDirection.Left)
        {
            moveDirection = MoveDirection.Right;
        }
        else if(moveDirection == MoveDirection.Right)
        {
            moveDirection = MoveDirection.Left;
        }
        moveCounter = 0;
    }

    // On collision with a player, bounce the player back.
    private void HitPlayer()
    {
        collisionVector = RL_EnemyManager.Instance.playerRigidbody2D.position - rb.position;
        collisionVector = collisionVector.normalized;
        if(Mathf.Abs(collisionVector.x )< 0.5f)
        {
            collisionVector.x = 0.5f * Mathf.Sign(collisionVector.x);
        }
        RL_EnemyManager.Instance.playerRigidbody2D.AddForce(collisionVector * forceMultiplier);
        RL_EnemyManager.Instance.playerStats.DamagePlayer(damage);
    }

    public void UpdateHealth()
    {
        if(currentHealth <= 0)
        {
            currentEnemyState = EnemyState.Dead;
        }
    }

    public void DamageEnemy(int dmg)
    {
        currentHealth -= dmg;
    }

    private void MakeDead()
    {
        currentEnemyState = EnemyState.Destroy;
    }
}
