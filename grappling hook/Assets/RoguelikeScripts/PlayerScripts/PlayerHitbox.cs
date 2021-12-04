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
            // Collides with the weapon. The parent is the offset, and then the next parent is the enemy.
            GameObject enemyGO = collision.gameObject.transform.parent.parent.gameObject;
            enemyAttack = enemyGO.GetComponent<EnemyAttack>();
            if (enemyAttack && enemyAttack.IsInDamageDealingPhase)
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
