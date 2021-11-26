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
        if(col.transform.gameObject.tag != "Enemy")
        {
            Die();
        }
    }

    // TODO This is probably bad to have this per-object
    // Would rather have this all in one manager, so if we have lots of bullets,
    // we don't have too many updates.
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
