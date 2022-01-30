// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PlayerParry : MonoBehaviour
// {
//     [SerializeField] private PlayerHitbox playerHitbox;
//     [SerializeField] private PlayerStats playerStats;
//
//     //private bool isInvincible = false; //?? maybe
//     
//     private float _parryTimer = 0f;
//
//     public void Parry()
//     {
//         // If we press F, and the player has been hit
//         if(playerHitbox.PlayerHit)
//         {
//             if (_parryTimer < playerHitbox.EnemyController.attackPlayerState.parryTimeLimit)
//             {
//                 PlayerParrySuccess();
//             }
//             else
//             {
//                 PlayerParryFail();
//             }
//         }
//         else
//         {
//             // else, punish player for bad parry for v short amount of time??
//             PlayerParryNothing();
//         }
//     }
//     
//     private void Update()
//     {
//         if (!playerHitbox.PlayerHit)
//         {
//             return;
//         }
//         
//         _parryTimer += Time.deltaTime;
//         
//         if(_parryTimer > playerHitbox.EnemyController.attackPlayerState.parryTimeLimit)
//         {
//             PlayerHit();
//         }
//     }
//
//     private void SetBackToWhite()
//     {
//         GetComponent<SpriteRenderer>().color = Color.white;
//     }
//     
//     private void PlayerHit()
//     {
//         Debug.Log("Player hit!");
//         playerStats.DamagePlayer(playerHitbox.EnemyController.attackPlayerState.attackDamage);
//         ResetParryState();
//     }
//
//     private void PlayerParrySuccess()
//     {
//         Debug.Log("Parry!");
//         GetComponent<SpriteRenderer>().color = Color.green;
//         SetBackToWhite();
//         // Rebound the enemy!
//         //playerHitbox.enemyAttack.ParryTween();
//
//         // TODO put back in
//         //playerHitbox.EnemyController.Parried();
//         playerHitbox.EnemyController.attackPlayerState.parried = true;//This could be an event?
//
//         ResetParryState();
//     }
//
//     private void PlayerParryFail()
//     {
//         Debug.Log("Hit: parried too late!");
//         playerStats.DamagePlayer(playerHitbox.EnemyController.attackPlayerState.attackDamage);
//         ResetParryState();
//     }
//
//     private void PlayerParryNothing()
//     {
//         Debug.Log("No hit, no parry!");
//         GetComponent<SpriteRenderer>().color = Color.yellow;
//         SetBackToWhite();
//         ResetParryState();
//     }
//
//     private void ResetParryState()
//     {
//         playerHitbox.ResetPlayerHitbox();
//         _parryTimer = 0f;
//     }
// }
