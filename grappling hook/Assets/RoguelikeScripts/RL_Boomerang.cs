using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RL_Boomerang : MonoBehaviour
{
    [SerializeField] private Transform boomerangTransform;
    [SerializeField] private RL_BoomerangCollision boomerangCollision;
    [SerializeField] private RL_PlayerController playerController;

    [SerializeField] private float boomerangSpeed;
    [SerializeField] private float boomerangRange;
    [SerializeField] private float boomerangUseTimeLimit = 0.25f;
    private float boomerangUseTimer = 0f;

    private Vector3 initialThrowPosition;
    private RL_PlayerController.FacingDirection initialThrowDirection;

    enum BoomerangState
    {
        None,
        BoomerangOut,
        BoomerangIn,
        End
    }
    BoomerangState boomerangState = BoomerangState.None;

    private void Awake()
    {
        boomerangUseTimer = boomerangUseTimeLimit + 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Mouse0) && boomerangState == BoomerangState.None && boomerangUseTimer > boomerangUseTimeLimit)
        {
            boomerangState = BoomerangState.BoomerangOut;
            boomerangTransform.position = playerController.gameObject.transform.position;
            initialThrowPosition = boomerangTransform.position;
            initialThrowDirection = playerController.GetFacingDirection();
            boomerangTransform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            boomerangUseTimer = 0f;
        }
        if( boomerangState == BoomerangState.None)
        {
            if(boomerangUseTimer < boomerangUseTimeLimit)
            {
                boomerangUseTimer += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if(boomerangState == BoomerangState.BoomerangOut)
        {
            boomerangTransform.position += new Vector3(1, 0, 0) * boomerangSpeed * (int)initialThrowDirection * Time.deltaTime;
            if (Mathf.Abs(boomerangTransform.position.sqrMagnitude - initialThrowPosition.sqrMagnitude)
                > boomerangRange * boomerangRange
                || boomerangCollision.CollisionHit)
            {
                boomerangState = BoomerangState.BoomerangIn;
                boomerangCollision.CollisionHit = false;
            }
        }
        else if( boomerangState == BoomerangState.BoomerangIn )
        {
            boomerangTransform.position += new Vector3(1, 0, 0) * boomerangSpeed * -(int)initialThrowDirection * Time.deltaTime;
        }


        if (initialThrowDirection == RL_PlayerController.FacingDirection.Left)
        {
            if (boomerangTransform.position.x >= playerController.transform.position.x)
            {
                boomerangState = BoomerangState.End;
            }
        }
        else
        {
            if (boomerangTransform.position.x <= playerController.transform.position.x)
            {
                boomerangState = BoomerangState.End;
            }
        }


        if (boomerangState == BoomerangState.End)
        {
            boomerangTransform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            boomerangState = BoomerangState.None;
        }
    }



}
