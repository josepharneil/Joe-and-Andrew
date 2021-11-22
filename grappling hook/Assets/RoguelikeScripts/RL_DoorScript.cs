using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_DoorScript : MonoBehaviour
{

    private RL_EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = RL_EnemyManager.Instance; ;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyManager.transform.childCount == 0f)
        {
            Destroy(gameObject);
        }
    }
}
