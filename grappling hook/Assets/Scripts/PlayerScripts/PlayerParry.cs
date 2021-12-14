using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [SerializeField] private PlayerHitbox playerHitbox;
    [SerializeField] private PlayerStats playerStats;

    //private bool isInvincible = false; //?? maybe

    enum ParryState
    {
        None,
        PlayerHit,// Player hits
        ParrySuccess, // Player hit, and parries away
        ParryFail,// Player hit, and parries too late
        ParryNothing // Player parries nothing.
    }
    ParryState parryState = ParryState.None;

    private float parryTimer = 0f;

    private void Update()
    {
        if( playerHitbox.PlayerHit )
        {
            parryTimer += Time.deltaTime;

            if(parryTimer > playerHitbox.EnemyController.attackData.ParryTimeLimit)
            {
                parryState = ParryState.PlayerHit;
            }
        }

        // Temp: E for parry
        if( Input.GetKeyDown(KeyCode.E) )
        {
            // If we press F, and the player has been hit
            if(playerHitbox.PlayerHit)
            {
                if( parryTimer < playerHitbox.EnemyController.attackData.ParryTimeLimit)
                {
                    parryState = ParryState.ParrySuccess;
                }
                else
                {
                    parryState = ParryState.ParryFail;
                }
            }
            else
            {
                // else, punish player for bad parry for v short amount of time??
                parryState = ParryState.ParryNothing;
            }


        }


        CheckParryState();

    }

    private IEnumerator SetBackToWhite()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;

    }

    private void CheckParryState()
    {
        switch (parryState)
        {
            case ParryState.None:
                {
                    break;
                }
            case ParryState.PlayerHit:
                {
                    PlayerHit();
                    break;
                }
            case ParryState.ParrySuccess:
                {
                    PlayerParrySuccess();
                    break;
                }
            case ParryState.ParryFail:
                {
                    PlayerParryFail();
                    break;
                }
            case ParryState.ParryNothing:
                {
                    PlayerParryNothing();
                    break;
                }
        }
    }

    private void PlayerHit()
    {
        Debug.Log("Player hit!");
        playerStats.DamagePlayer(playerHitbox.EnemyController.attackData.AttackDamage);
        ResetParryState();
    }

    private void PlayerParrySuccess()
    {
        Debug.Log("Parry!");
        GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(SetBackToWhite());
        // Rebound the enemy!
        //playerHitbox.enemyAttack.ParryTween();

        // TODO put back in
        //playerHitbox.EnemyController.Parried();
        playerHitbox.EnemyController.attackData.Parried = true;

        ResetParryState();
    }

    private void PlayerParryFail()
    {
        Debug.Log("Hit: parried too late!");
        playerStats.DamagePlayer(playerHitbox.EnemyController.attackData.AttackDamage);
        ResetParryState();
    }

    private void PlayerParryNothing()
    {
        Debug.Log("No hit, no parry!");
        GetComponent<SpriteRenderer>().color = Color.yellow;
        StartCoroutine(SetBackToWhite());
        ResetParryState();
    }

    private void ResetParryState()
    {
        playerHitbox.ResetPlayerHitbox();
        parryTimer = 0f;
        parryState = ParryState.None;
    }
}
