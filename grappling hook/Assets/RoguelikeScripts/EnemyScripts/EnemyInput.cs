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

    private void Update()
    {
        // Player not found
        RaycastCheckToPlayer();

        // Check initial position (hardcoded)
        IsAtOriginalPosition = transform.position.x == 15;
    }

    private void RaycastCheckToPlayer()
    {
        Debug.DrawRay(transform.position, playerTransform.position - transform.position);

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.layerMask = groundAndPlayerMask;
        contactFilter2D.useLayerMask = true;
        Physics2D.Raycast(transform.position, playerTransform.position - transform.position, contactFilter2D, raycastHitsTowardsPlayer);
    }

    public bool IsRaycastHittingPlayer()
    {
        // Check if we've hit the player first (before ground)
        if (raycastHitsTowardsPlayer.Count == 0)
        {
            return false;
        }
        RaycastHit2D playerHit = raycastHitsTowardsPlayer[0];
        if (playerHit.transform.gameObject.tag != "Player")
        {
            return false;
        }
        return true;
    }

    public bool IsInAttackRange()
    {
        return raycastHitsTowardsPlayer[0].distance < attackMaxDistance;
    }

    public bool IsInChaseRange()
    {
        return raycastHitsTowardsPlayer[0].distance < chaseMaxDistance;
    }

    public bool IsInSightRange()
    {
        return raycastHitsTowardsPlayer[0].distance < sightMaxDistance;
    }
}
