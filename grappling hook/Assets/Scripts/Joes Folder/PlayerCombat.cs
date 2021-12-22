using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Serializable]
    private struct AttackInfo
    {
        public Transform hitBoxPosition;
        public int damage;
        public float radius;
    }

    [SerializeField] private AttackInfo attackInfo1;
    [SerializeField] private AttackInfo attackInfo2;

    
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private PlayerControllerCombatScene controller; //bad circular reference

    public void CheckAttackHitBox(int attackIndex)
    {
        ref AttackInfo attackInfo = ref attackInfo1;
        switch (attackIndex)
        {
            case 0:
                break;
            case 1:
                attackInfo = ref attackInfo2;
                break;
            default:
                Debug.LogError("No such attack index");
                break;
        }
        Vector2 overlapCirclePosition;
        if (controller.facingDirection == PlayerControllerCombatScene.FacingDirection.Left)
        {
            var localPosition = attackInfo.hitBoxPosition.localPosition;
            overlapCirclePosition = (Vector2)transform.position + new Vector2(-localPosition.x, localPosition.y );
        }
        else
        {
            overlapCirclePosition = attackInfo.hitBoxPosition.position;
        }
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            layerMask = whatIsDamageable
        };
        List<Collider2D> detectedObjects = new List<Collider2D>();
        Physics2D.OverlapCircle(overlapCirclePosition, attackInfo.radius, contactFilter2D, detectedObjects);
        
        foreach (Collider2D coll in detectedObjects)
        {
            SandbagEnemy sandbagEnemy = coll.gameObject.GetComponent<SandbagEnemy>();
            if (sandbagEnemy)
            {
                sandbagEnemy.DamageThisEnemy( attackInfo.damage );
            }
            
            // Instantiate a hit particle here if we want
        }
    }

    private void OnDrawGizmos()
    {
        if (controller.facingDirection == PlayerControllerCombatScene.FacingDirection.Left)
        {
            var localPosition = attackInfo1.hitBoxPosition.localPosition;
            Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y );
            Gizmos.DrawWireSphere(position, attackInfo1.radius);
        }
        else
        {
            Gizmos.DrawWireSphere(attackInfo1.hitBoxPosition.position, attackInfo1.radius);
        }
        
        if (controller.facingDirection == PlayerControllerCombatScene.FacingDirection.Left)
        {
            var localPosition = attackInfo2.hitBoxPosition.localPosition;
            Vector3 position = transform.position + new Vector3(-localPosition.x, localPosition.y );
            Gizmos.DrawWireSphere(position, attackInfo2.radius);
        }
        else
        {
            Gizmos.DrawWireSphere(attackInfo2.hitBoxPosition.position, attackInfo2.radius);
        }
    }
}
