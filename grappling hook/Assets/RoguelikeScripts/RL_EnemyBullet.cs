using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_EnemyBullet : MonoBehaviour
{
    [Header("Config")]
    public float shotDuration, damage;

    //public GameObject diePeffect;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountDownTimer());
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("collisionEnter");
        Die();
    }


    IEnumerator CountDownTimer()
    {
        yield return new WaitForSeconds(shotDuration);

        Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
