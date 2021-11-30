using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public bool PlayerHit = false;

    public EnemyAttack enemyAttack = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag( "EnemyAttack" ) )
        {
            Debug.Log("Hitbox entered");
            PlayerHit = true;
            enemyAttack = collision.GetComponent<EnemyAttack>();
        }
    }

    public void ResetPlayerHitbox()
    {
        PlayerHit = false;
        enemyAttack = null;
    }
}
