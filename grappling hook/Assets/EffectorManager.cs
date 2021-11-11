using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectorManager : MonoBehaviour
{
    private static EffectorManager _instance;

    public static EffectorManager Instance { get { return _instance; } }

    public enum EffectorType
    {
        ConstantVelocity,
        Reflect,
        HorizontalVelocity,
    }

    [SerializeField] private MyPlayerController playerController;
    [SerializeField] private Rigidbody2D playerRB;

    [HideInInspector] public bool ConstantVelocityEffect = false;
    private Vector2 entryVelocity;

    [HideInInspector] public bool ReflectEffect = false;

    [HideInInspector] public bool HorizontalVelocityEffect = false;
    private Vector2 entryVelocityX;

     

    private void Awake()
    {
        if( _instance != null && _instance != this )
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ApplyEffect(EffectorType effectorType)
    {
        switch(effectorType)
        {
            case EffectorType.ConstantVelocity:
                ApplyEffectConstantVelocity();
                break;
            case EffectorType.Reflect:
                ApplyEffectReflect();
                break
            case EffectorType.HorizontalVelocity:
                ApplyEffectHorizontalVelocity();
                break;
        }
    }

    public void RemoveEffect(EffectorType effectorType)
    {
        switch (effectorType)
        {
            case EffectorType.ConstantVelocity:
                RemoveEffectConstantVelocity();
                break;
            case EffectorType.Reflect:
                RemoveEffectReflect();
                break;
            case EffectorType.HorizontalVelocity:
                RemoveEffectHorizontalVelocity();
                break;
        }
    }

    private void Update()
    {
        if (ConstantVelocityEffect)
        {
            playerRB.velocity = entryVelocity;
        }
        else if (HorizontalVelocityEffect)
        {
            //sets the velocity of the player to the x component of the entry velocity
            //locks the Y position because gravity was still applying while it was moving along
            //there's some weirdo stuff happening with rotation at the end that I don't get tho
            playerRB.velocity = entryVelocityX;
            playerRB.constraints = RigidbodyConstraints2D.FreezePositionY| RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public bool CurrentEffectsDisablePlayerInput()
    {
        return ReflectEffect || ConstantVelocityEffect || HorizontalVelocityEffect;
    }

    private void ApplyEffectConstantVelocity()
    {
        ConstantVelocityEffect = true;
        entryVelocity = playerRB.velocity;
        if(entryVelocity.magnitude < 10)
        {
            entryVelocity = entryVelocity.normalized * 10;
        }
    }

    private void RemoveEffectConstantVelocity()
    {
        ConstantVelocityEffect = false;
    }


    private void ApplyEffectReflect()
    {
        ReflectEffect = true;
        playerRB.velocity = -playerRB.velocity;
        playerRB.AddForce(playerRB.velocity * 5, ForceMode2D.Impulse);
    }

    private void RemoveEffectReflect()
    {
        ReflectEffect = false;
    }

    private void ApplyEffectHorizontalVelocity()
    {
        HorizontalVelocityEffect = true;
        entryVelocity = playerRB.velocity;
        entryVelocityX = new Vector2(playerRB.velocity.x,0f);
        
    }
    private void RemoveEffectHorizontalVelocity()
    {
        HorizontalVelocityEffect = false;
        playerRB.velocity = entryVelocity;
        playerRB.constraints = RigidbodyConstraints2D.None;
    }


}
