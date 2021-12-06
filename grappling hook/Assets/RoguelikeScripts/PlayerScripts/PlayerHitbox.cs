using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public bool PlayerHit = false;

    public EnemyController EnemyController = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag( "EnemyAttack" ) )
        {
            Debug.Log("Hitbox entered");
            // Collides with the weapon. The parent is the offset, and then the next parent is the enemy.
            GameObject enemyGO = collision.gameObject.transform.parent.parent.gameObject;
            EnemyController = enemyGO.GetComponent<EnemyController>();
            if (EnemyController != null && EnemyController.attackData.IsInDamageDealingPhase)
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
        EnemyController = null;
    }
}
