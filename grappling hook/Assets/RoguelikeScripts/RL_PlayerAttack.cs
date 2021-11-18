using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float attackDuration = 0.005f;
    [SerializeField] private BoxCollider2D weaponCollider;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float attackTimer = float.MaxValue;
    [SerializeField] private SpriteRenderer weaponRender;
    [SerializeField] private RL_PlayerController playerController;


    [Header("Debug")]
    [SerializeField] private bool isAttackingThisUpdate;
    [SerializeField] private bool isAttackStart;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isAttackEnd;


    
    public Transform weaponTransform;
    private float weaponInitialX;
    private float weaponInitialY;
    private Quaternion weaponInitialRotation;

    void Awake()
    {
        weaponRender.enabled = false;
        weaponInitialRotation = weaponTransform.rotation;
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
        weaponRender.enabled = true;
        isAttackStart = false;
    }

    private void ApplyUpdateAttack()
    {
     
        weaponTransform.Rotate(new Vector3(0,0,1),-6f);
        //weaponCollider.transform.position = new Vector3(weaponCollider.transform.position.x + 0.01f, weaponCollider.transform.position.y - 0.005f, transform.position.z);
        attackTimer += Time.deltaTime;
        if(attackTimer >= attackDuration)
        {
            isAttackEnd = true;
            isAttacking = false;
        }
    }

    private void ApplyEndAttack()
    {
        weaponRender.enabled = false;
        attackTimer = float.MaxValue;
        isAttackEnd = false;
        weaponTransform.rotation = weaponInitialRotation;
    }
}
