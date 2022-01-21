using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_MovingPlatform : MonoBehaviour
{
    [Header("Config")]
    public HorizontalDirection xDirection = HorizontalDirection.right;
    public VerticalDirection yDirection = VerticalDirection.up;
    public float xTranslation;
    public float yTranslation;
    public float xSpeed;
    public float ySpeed;

    [Header("Debug")]
    [SerializeField] private float xCounter;
    [SerializeField] private float yCounter;

    private Transform gameObjectTransform;

    public enum HorizontalDirection
    {
        left = -1,
        none = 0,
        right = 1,
    }
    public enum VerticalDirection
    {
        up = 1,
        none = 0,
        down = -1,
    }
    void Awake()
    {
        gameObjectTransform = gameObject.transform;
    }
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void ChangeDirection()
    {
        if(xDirection == HorizontalDirection.left)
        {
            xDirection = HorizontalDirection.right;
            xCounter = 0;
        }else if(xDirection == HorizontalDirection.right)
        {
            xDirection = HorizontalDirection.left;
            xCounter = 0;
        }

        if(yDirection == VerticalDirection.up)
        {
            yDirection = VerticalDirection.down;
            yCounter = 0;
        }
        else if (yDirection == VerticalDirection.down)
        {
            yDirection = VerticalDirection.up;
            yCounter = 0;
        }
    }

    void Move()
    {
        gameObjectTransform.Translate(new Vector3(xSpeed*(int)xDirection, ySpeed*(int)yDirection, 0));
        if (xDirection != HorizontalDirection.none)
        {
            xCounter += xSpeed;
        }
        if(yDirection != VerticalDirection.none)
        {
            yCounter += ySpeed;
        }
        if((xCounter >=xTranslation &&xDirection!=HorizontalDirection.none) || (yCounter >= yTranslation && yDirection!=VerticalDirection.none))
        {
            ChangeDirection();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        col.gameObject.transform.SetParent(gameObject.transform, true);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        col.gameObject.transform.parent = null;
    }
}
