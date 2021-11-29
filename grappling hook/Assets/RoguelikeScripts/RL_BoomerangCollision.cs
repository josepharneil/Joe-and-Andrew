using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_BoomerangCollision : MonoBehaviour
{
    public bool CollisionHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            CollisionHit = true;
        }
    }
}
