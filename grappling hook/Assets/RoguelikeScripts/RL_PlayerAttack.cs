using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float attackDuration = 0.02f;
    [SerializeField] private BoxCollider2D weaponCollider;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float attackTimer = float.MaxValue;

    [Header("Debug")]
    [SerializeField] private bool isAttackingThisUpdate;
    [SerializeField] private bool isAttackStart;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isAttackEnd;


    //private Raycast hit;
    public GameObject weapon;
    private float weaponInitialX;
    private float weaponInitialY;
    private Quaternion weaponInitialRotation;

    void Awake()
    {
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        
        weaponInitialX = weaponCollider.transform.localPosition.x;
        weaponInitialY = weaponCollider.transform.localPosition.y;
        weaponInitialRotation = weaponCollider.transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        HandleAttackInput();
    }

    void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttackStart = true;
            attackTimer = 0f;
        }
    }
    void FixedUpdate()
    {
        ApplyAttack();
    }

    private void ApplyAttack()
    {
        UpdateAttackState();
        if (isAttackStart)
        {
            ApplyStartAttack();
            
        }
        UpdateAttackState();
        if (isAttacking)
        {
            ApplyUpdateAttack();
        }

       if (isAttackEnd)
        {
            ApplyEndAttack();
        }
    }

    private void UpdateAttackState()
    {
        isAttackingThisUpdate = attackTimer < attackDuration;
        if (isAttacking && !isAttackingThisUpdate)
        {
            isAttackEnd = true;
        }
        isAttacking = isAttackingThisUpdate;
        
    }

    private void ApplyStartAttack()
    {
        weapon.GetComponent<SpriteRenderer>().enabled = true;
        isAttackStart = false;
    }

    private void ApplyUpdateAttack()
    {
        
        
        weaponCollider.transform.Rotate(new Vector3(0,0,1),-1.5f);
        weaponCollider.transform.position = new Vector3(weaponCollider.transform.position.x + 0.02f, weaponCollider.transform.position.y - 0.02f, transform.position.z);
        attackTimer += Time.deltaTime;
        if(attackTimer >= attackDuration)
        {
            isAttackEnd = true;
            isAttacking = false;
        }
    }

    private void ApplyEndAttack()
    {
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        attackTimer = float.MaxValue;
        isAttackEnd = false;
        weaponCollider.transform.localPosition = new Vector3(weaponInitialX, weaponInitialY, 0);
        weaponCollider.transform.rotation = weaponInitialRotation;
    }
}
