using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_EnemyShooter : MonoBehaviour
{
    //using bits from https://www.youtube.com/watch?v=Htw2f2eqLFk

    [Header("Setup")]
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Transform enemyPosition;
    [SerializeField] private Transform shootPosition;
    [SerializeField] public GameObject bullet;

    [Header("Config")]
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float range;
    [SerializeField] private float shotSpeed;

    [Header("Dubg")]
    [SerializeField] private float distanceToPlayer;
    

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector2.Distance(playerRB.position, enemyPosition.position);
        if (distanceToPlayer <= range)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        while (distanceToPlayer <= range)
        {
            Vector2 shotVector = (Vector2)playerRB.position - (Vector2)shootPosition.position;
            shotVector.Normalize();
            yield return new WaitForSeconds(timeBetweenShots);
            GameObject newBullet = Instantiate(bullet, shootPosition.position, Quaternion.identity, enemyPosition);
            newBullet.GetComponent<Rigidbody2D>().velocity = shotVector * shotSpeed;
        }

    }
}
