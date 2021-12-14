using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_EnemyShooter : MonoBehaviour
{
    //using bits from https://www.youtube.com/watch?v=Htw2f2eqLFk

    [Header("Setup")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform shootTransform;
    [SerializeField] public GameObject bullet;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Config")]
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float range;
    [SerializeField] private float shotSpeed;

    [Header("Debug")]
    [SerializeField] private bool canShoot;

    //public RaycastHit2D hitPlayer;
    public float raycastDistance;
    public LayerMask groundAndPlayerMasks;

    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(shootTransform.position, (playerTransform.position - shootTransform.position) * (range/Vector2.Distance(playerTransform.position, shootTransform.position)));
        if(canShoot)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.layerMask = groundAndPlayerMasks;
            contactFilter.useLayerMask = true;

            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            // TODO Could use manager to get transform.
            Physics2D.Raycast(shootTransform.position, playerTransform.position - shootTransform.position, contactFilter, hits, range);

            bool playerIsShootable = false;
            // 0th hit is the enemy itself (THIS DEPENDS ON THE POSITIONING OF THE SHOOT TRANSFORM CAREFUL)
            if(hits.Count > 1)
            {
                if (hits[1].transform.gameObject.tag == "Player")
                {
                    playerIsShootable = true;
                    spriteRenderer.color = Color.red;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }
            if(playerIsShootable)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        //AK: Can shoot stops the bullet from being generated every frame
        canShoot = false;
        yield return new WaitForSeconds(timeBetweenShots);

        Vector2 shotVector = (Vector2)playerTransform.position - (Vector2)shootTransform.position;
        shotVector.Normalize();
        GameObject newBullet = Instantiate(bullet, shootTransform.position, Quaternion.identity, transform);
        newBullet.GetComponent<Rigidbody2D>().velocity = shotVector * shotSpeed;
        canShoot = true;
    }
}
