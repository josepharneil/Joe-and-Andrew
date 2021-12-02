using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_WeaponHit : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int damage =3;
    [SerializeField] private int force = 3;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject colliderObject = collider.gameObject;
        if (colliderObject.tag == "Enemy")
        {
            RL_EnemyBullet enemyBullet = colliderObject.GetComponent<RL_EnemyBullet>();
            RL_SimpleEnemyBehaviour simpleEnemyBehaviour = colliderObject.GetComponent<RL_SimpleEnemyBehaviour>();
            if (simpleEnemyBehaviour)
            {
                // Make dead. Enemy scripts will handle from there what to do.
                //simpleEnemyBehaviour.CurrentEnemyState = RL_SimpleEnemyBehaviour.EnemyState.Dead;
                simpleEnemyBehaviour.DamageEnemy(damage);
                simpleEnemyBehaviour.GetHit(force, gameObject.transform.position);

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
