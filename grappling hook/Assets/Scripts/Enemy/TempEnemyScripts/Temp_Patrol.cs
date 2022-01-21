using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// using this to test out some simple enemy animations,
/// can be deleted once we've got bolt up and runing
/// </summary>
public class Temp_Patrol : MonoBehaviour
{
    [SerializeField] private float patrolDistanceX;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveSpeed;

    private float distanceMoved = 0f;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    bool _isAttacking = false;

    private FacingDirection _currentPatrolDirection = FacingDirection.Right;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        AttackCheck();
        Patrol();
    }

    void Patrol()
    {
        if (!_isAttacking)
        {
            gameObject.transform.Translate(new Vector2((int)_currentPatrolDirection * moveSpeed * Time.deltaTime, 0f));
            distanceMoved += (int)_currentPatrolDirection * moveSpeed * Time.deltaTime;
            if (Mathf.Abs(distanceMoved) >= 10f)
            {
                ChangeDirection();
            }
        }
    }

    void ChangeDirection()
    {
        distanceMoved = 0f;
        if (_currentPatrolDirection == FacingDirection.Left)
        {
            _currentPatrolDirection = FacingDirection.Right;
            spriteRenderer.flipX=false;
            return;
        }
        else if (_currentPatrolDirection == FacingDirection.Right)
        {
            _currentPatrolDirection = FacingDirection.Left;
            spriteRenderer.flipX = true;
            return;
        }
    }
    void AttackCheck()
    {
        if (Vector2.Distance(playerTransform.position, gameObject.transform.position)<=5f)
        {
            _isAttacking = true;
            Attack();

        }
    }
    void Attack()
    {
        animator.SetTrigger("attackTrigger");
        _isAttacking = false;
    }
}
