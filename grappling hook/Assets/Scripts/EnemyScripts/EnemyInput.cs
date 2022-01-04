using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : MonoBehaviour
{
    // Some data:
    // Should be a scriptable object?
    [Header("Health")]
    public EnemyHealth enemyHealth;

    [Header("Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask groundAndPlayerMask;


    // Could move this to a different object with no functions.
    [Header("Enemy Info")]
    [SerializeField] private float attackMaxDistance = 1.5f;
    [SerializeField] private float chaseMaxDistance = 10f;
    [SerializeField] private float sightMaxDistance = 15f;

    private List<RaycastHit2D> raycastHitsTowardsPlayer = new List<RaycastHit2D>();

    public bool IsAtOriginalPosition = false;
    public Vector2 originalPosition;

    private void Start()
    {
        originalPosition = gameObject.transform.position;
    }

    private void Update()
    {
        // Player not found
        RaycastCheckToPlayer();

        // Check initial position (hardcoded)
        IsAtOriginalPosition = transform.position.x == originalPosition.x;
    }

    private void RaycastCheckToPlayer()
    {
        var position = transform.position;
        var playerPosition = playerTransform.position;
        Debug.DrawRay(position, playerPosition - position);

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.layerMask = groundAndPlayerMask;
        contactFilter2D.useLayerMask = true;
        Physics2D.Raycast(position, playerPosition - position, contactFilter2D, raycastHitsTowardsPlayer);
    }

    public bool IsRaycastHittingPlayer()
    {
        RaycastHit2D playerHit = raycastHitsTowardsPlayer[0];
        if (playerHit.transform.gameObject.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }
    //TODO AK: make a single method for if raycast is hitting the player
    //We will need a few differnt ones for enemy types, but thats fine
    public bool PlayerIsInAttackRange()
    {
        if (raycastHitsTowardsPlayer.Count == 0||!IsRaycastHittingPlayer())
        {
            return false;
        }
        
        return raycastHitsTowardsPlayer[0].distance < attackMaxDistance;
    }

    public bool PlayerIsInChaseRange()
    {
        if (raycastHitsTowardsPlayer.Count == 0||!IsRaycastHittingPlayer())
        {
            return false;
        }
        return raycastHitsTowardsPlayer[0].distance < chaseMaxDistance;
    }

    public bool PlayerIsInSightRange()
    {
        if (raycastHitsTowardsPlayer.Count == 0 || !IsRaycastHittingPlayer()) 
        {
            return false;
        }
        RaycastHit2D playerHit = raycastHitsTowardsPlayer[0];
        if (playerHit.transform.gameObject.CompareTag("Player")&&playerHit.distance<sightMaxDistance)
        {
            return true;
        }
        return false;
    }
}
