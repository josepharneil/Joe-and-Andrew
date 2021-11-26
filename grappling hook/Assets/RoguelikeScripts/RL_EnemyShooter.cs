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
    [SerializeField] private bool canShoot;
    [SerializeField] private bool startRoutine;

    public RaycastHit2D hitPlayer;
    public float raycastDistance;
    public LayerMask layerMask;

    void Start()
    {
        canShoot = true;
        startRoutine = false;
    }

    // Update is called once per frame
    void Update()
    {

        distanceToPlayer = Vector2.Distance(playerRB.position, shootPosition.position);
        
        //AK: the range doesn't seem to apply properly
        // it also seems to completely shut down if the layer is set to ground
        hitPlayer = Physics2D.Raycast(shootPosition.position, playerRB.position, range);
        raycastDistance = hitPlayer.distance;
        if (!Physics2D.Raycast(shootPosition.position, playerRB.position, range, layerMask) &&  hitPlayer && canShoot)
        {
            StartCoroutine(Shoot());
        }
        
    }

    IEnumerator Shoot()
    {
        //AK: Can shoot stops the bullet from being generated every frame
        startRoutine = true;
        canShoot = false;
        yield return new WaitForSeconds(timeBetweenShots);
        Vector2 shotVector = (Vector2)playerRB.position - (Vector2)shootPosition.position;
        shotVector.Normalize();
        GameObject newBullet = Instantiate(bullet, shootPosition.position, Quaternion.identity, enemyPosition);
        newBullet.GetComponent<Rigidbody2D>().velocity = shotVector * shotSpeed;
        canShoot = true;
    }
}
