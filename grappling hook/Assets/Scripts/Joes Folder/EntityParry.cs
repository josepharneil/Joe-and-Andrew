using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityParry : MonoBehaviour
{
    [SerializeField] private PlayerInputs inputs;
    [SerializeField] private LayerMask whatIsParryable;
    [SerializeField] private float parryRadius = 1f;
    
    public void CheckParry()
    {
        FacingDirection parryDirection = inputs.facingDirection;
        
        Vector2 overlapCirclePosition;
        if (parryDirection == FacingDirection.Left)
        {
            overlapCirclePosition = (Vector2)transform.position + new Vector2(-1, 0 );
        }
        else
        {
            overlapCirclePosition = (Vector2)transform.position + new Vector2(1, 0 );
        }
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            layerMask = whatIsParryable,
            useLayerMask = true
        };
        List<Collider2D> detectedObjects = new List<Collider2D>();
        Physics2D.OverlapCircle(overlapCirclePosition, parryRadius, contactFilter2D, detectedObjects);

        foreach (Collider2D coll in detectedObjects)
        {
            EntityParryable entityParryable = coll.gameObject.GetComponent<EntityParryable>();
            if (entityParryable && entityParryable.isCurrentlyParryable)
            {
                entityParryable.Parry();
                break;
            }

            // Instantiate a hit particle here if we want
        }
    }

    private void OnDrawGizmos()
    {
        if (inputs.facingDirection == FacingDirection.Left)
        {
            Vector3 position = transform.position + new Vector3(-1, 0);
            Gizmos.DrawWireSphere(position, parryRadius);
        }
        else
        {
            Vector3 position = transform.position + new Vector3(1, 0);
            Gizmos.DrawWireSphere(position, parryRadius);
        }
    }
}
