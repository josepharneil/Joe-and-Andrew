using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_EnemyManager : MonoBehaviour
{
    // Note: we could do inheritance or composition?
    [SerializeField] private List<RL_SimpleEnemyBehaviour> enemies;

    // Singleton instance.
    private static RL_EnemyManager _instance;
    public static RL_EnemyManager Instance { get { return _instance; } }

    public Rigidbody2D playerRigidbody2D;
    public RL_PlayerStats playerStats;

    private void Awake()
    {
        // Singleton.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        enemies.Clear();
        for ( int childIndex = 0; childIndex < transform.childCount; childIndex++ )
        {
            RL_SimpleEnemyBehaviour enemyBehaviour = transform.GetChild(childIndex).GetComponent<RL_SimpleEnemyBehaviour>();
            if (enemyBehaviour)
            {
                enemies.Add(enemyBehaviour);
            }
        }
    }

    private void FixedUpdate()
    {
        List<int> enemyIndexesToDestroy = new List<int>();
        for( int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++ )
        {
            RL_SimpleEnemyBehaviour enemyBehaviour = enemies[enemyIndex];
            if (enemyBehaviour.CurrentEnemyState != RL_SimpleEnemyBehaviour.EnemyState.Destroy)
            {
                enemyBehaviour.EnemyUpdate();
            }
            else
            {
                enemyIndexesToDestroy.Add(enemyIndex);
            }
        }
        foreach( int enemyIndex in enemyIndexesToDestroy )
        {
            RL_SimpleEnemyBehaviour enemyBehaviour = enemies[enemyIndex];
            Destroy(enemyBehaviour.gameObject);
            enemies.RemoveAt(enemyIndex);
        }

    }
}
