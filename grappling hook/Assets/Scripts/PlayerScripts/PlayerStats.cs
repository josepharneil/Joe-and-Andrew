using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private IntVariable playerHealth;
    [SerializeField] private IntVariable collectables;
    // TODO Temp just teleport
    private Vector3 _initialSpawnPosition;

    private void Awake()
    {
        _initialSpawnPosition = transform.position;
    }

    private enum PlayerState
    {
        Alive,
        Dead
    }
    private PlayerState playerState = PlayerState.Alive;

    public void SetPlayerDead()
    {
        playerState = PlayerState.Dead;
        GetComponent<SpriteRenderer>().color = Color.black;
        //TODO Hacky
        ResetPlayer();
    }

    public void SetPlayerAlive()
    {
        playerState = PlayerState.Alive;
    }

    public bool IsPlayerDead()
    {
        return playerState == PlayerState.Dead;
    }

    public bool IsPlayerAlive()
    {
        return playerState == PlayerState.Alive;
    }

    private void ResetPlayer()
    {
        //StartCoroutine(WaitThenReset());
        transform.position = _initialSpawnPosition;
        playerHealth.RuntimeValue = playerHealth.InitialValue;
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<CapsuleCollider2D>().enabled = true;
        SetPlayerAlive();
    }

    IEnumerator WaitThenReset()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = _initialSpawnPosition;
        playerHealth.RuntimeValue = playerHealth.InitialValue;
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<CapsuleCollider2D>().enabled = true;
        SetPlayerAlive();
    }

    public void DamagePlayer( int damage )
    {
        playerHealth.RuntimeValue -= damage;
        if( playerHealth.RuntimeValue < 0)
        {
            playerHealth.RuntimeValue = 0;
        }
        if( playerHealth.RuntimeValue == 0)
        {
            SetPlayerDead();
        }
    }

    public void HealPlayer( int healAmount )
    {
        playerHealth.RuntimeValue += healAmount;
        if(playerHealth.RuntimeValue > playerHealth.InitialValue)
        {
            playerHealth.RuntimeValue = playerHealth.InitialValue;
        }
    }
}
