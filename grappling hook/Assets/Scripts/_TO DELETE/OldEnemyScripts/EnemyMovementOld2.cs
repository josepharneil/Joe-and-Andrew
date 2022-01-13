using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class EnemyMovementOld2 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask groundAndPlayerMasks;

    // Less than search, enemy has found player and moves towards player
    [SerializeField] private float searchRange = 5f;
    // Less than attack, enemy is in range to attack and start attacking
    [SerializeField] private float attackRange = 2f;

    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private EnemyHealth enemyHealth;

    public enum EnemyMovementState
    {
        Searching,
        Moving,
        Attacking
    }
    public EnemyMovementState enemyMovementState = EnemyMovementState.Searching;

    // Static for now, this whole event stuff going on is not well done.
    public static event Action<EnemyMovementState> OnEnemyMovementStateChanged;

    private void Awake()
    {
        Debug.Assert(searchRange > attackRange);
    }

    // Update is called once per frame
    void Update()
    {
        //if(enemyHealth.enemyState == EnemyHealth.EnemyState.Dead
        //    || enemyHealth.enemyState == EnemyHealth.EnemyState.Destroy)
        //{
        //    return;
        //}
        Debug.DrawRay(transform.position, player.transform.position - transform.position);

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.layerMask = groundAndPlayerMasks;
        contactFilter2D.useLayerMask = true;

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Physics2D.Raycast(transform.position, player.transform.position - transform.position, contactFilter2D, hits);

        // 0th hit is the enemy itself (THIS DEPENDS ON THE POSITIONING OF THE SHOOT TRANSFORM CAREFUL)
        if (hits.Count > 0)
        {
            RaycastHit2D maybePlayerHit = hits[0];
            if (maybePlayerHit.transform.gameObject.tag == "Player")
            {
                if( maybePlayerHit.distance < attackRange )
                {
                    ChangeEnemyState(EnemyMovementState.Attacking);
                }
                else if( maybePlayerHit.distance < searchRange )
                {
                    ChangeEnemyState(EnemyMovementState.Moving);
                }
                else
                {
                    ChangeEnemyState(EnemyMovementState.Searching);
                }
            }
        }


        if (enemyMovementState == EnemyMovementState.Moving)
        {
            if (player.transform.position.x < transform.position.x)
            {
                transform.position += new Vector3(-1, 0, 0) * Time.deltaTime * moveSpeed;
            }
            else
            {
                transform.position += new Vector3(1, 0, 0) * Time.deltaTime * moveSpeed;
            }
        }
    }

    private void ChangeEnemyState( EnemyMovementState newState )
    {
        if( enemyMovementState == newState)
        {
            return;
        }
        enemyMovementState = newState;

        switch(newState)
        {
            case EnemyMovementState.Searching:
                break;
            case EnemyMovementState.Moving:
                break;
            case EnemyMovementState.Attacking:
                break;
        }

        OnEnemyMovementStateChanged.Invoke(newState);
    }
}
