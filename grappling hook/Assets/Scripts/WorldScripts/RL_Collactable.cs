using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RL_Collactable : MonoBehaviour
{
    
    private RL_CollectableManager manager;

    void Start()
    {
        manager = RL_CollectableManager.Instance;
    }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            manager.CollectCollectable();
            Destroy(gameObject);
        }
    }
}
