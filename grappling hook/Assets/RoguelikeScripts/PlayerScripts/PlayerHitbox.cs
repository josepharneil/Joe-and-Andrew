using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class PlayerHitbox : MonoBehaviour
{
    public bool PlayerHit = false;

    public EnemyAttack enemyAttack = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag( "EnemyAttack" ) )
        {
            Debug.Log("Hitbox entered");
            enemyAttack = collision.GetComponent<EnemyAttack>();
            if(enemyAttack.IsAttacking)
            {
                PlayerHit = true;
            }
            else
            {
                ResetPlayerHitbox();
            }
        }
    }

    public void ResetPlayerHitbox()
    {
        PlayerHit = false;
        enemyAttack = null;
    }
}
