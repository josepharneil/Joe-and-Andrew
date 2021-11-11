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
    }

    [SerializeField] private MyPlayerController playerController;
    [SerializeField] private Rigidbody2D playerRB;
    [HideInInspector] public bool ConstantVelocityEffect = false;
    private Vector2 entryVelocity;

    [HideInInspector] public bool ReflectEffect = false;

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
        }
    }

    private void Update()
    {
        if (ConstantVelocityEffect)
        {
            playerRB.velocity = entryVelocity;
        }
    }

    public bool CurrentEffectsDisablePlayerInput()
    {
        return ReflectEffect || ConstantVelocityEffect;
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




}
