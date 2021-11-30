using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_WeaponHit : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject gameObject = collider.gameObject;
        if (gameObject.tag == "Enemy")
        {
            RL_EnemyBullet enemyBullet = gameObject.GetComponent<RL_EnemyBullet>();
            RL_SimpleEnemyBehaviour simpleEnemyBehaviour = gameObject.GetComponent<RL_SimpleEnemyBehaviour>();
            if (simpleEnemyBehaviour)
            {
                // Make dead. Enemy scripts will handle from there what to do.
                simpleEnemyBehaviour.CurrentEnemyState = RL_SimpleEnemyBehaviour.EnemyState.Dead;
            }else if(enemyBullet)
            {

                Debug.LogError("Bullet Hit");
                enemyBullet.Die();
            }
            else
            {
                Debug.LogError("Expected a simple enemy behaviour...");
            }
        }
    }
}
