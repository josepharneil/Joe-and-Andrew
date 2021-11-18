using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_WeaponHit : MonoBehaviour
{
    
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider)
    {
   
        if (collider.gameObject.tag == "Enemy")
        {
            Destroy(collider.gameObject);
        }
    }
}
