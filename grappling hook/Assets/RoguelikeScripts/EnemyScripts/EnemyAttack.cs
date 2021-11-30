using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class EnemyAttack : MonoBehaviour
{
    // public bool IsParryable ??
    public int AttackDamage = 1;
    public float Speed = 5f;
    public float ParryTimeLimit = 0.2f;

    [SerializeField] private Transform limit0;
    [SerializeField] private Transform limit1;

    // Start is called before the first frame update
    void Awake()
    {
        transform.position = limit0.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * Speed * Time.deltaTime;
        if( transform.position.y < limit1.position.y )
        {
            ResetEnemyAttack();
        }
    }

    public void ResetEnemyAttack()
    {
        transform.position = limit0.position;
    }
}
