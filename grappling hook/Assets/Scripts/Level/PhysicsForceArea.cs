using System;
using Entity;
using Player;
using UnityEngine;

namespace Level
{
    public class PhysicsForceArea : MonoBehaviour
    {
        [SerializeField] private Vector2 _forceDirection = Vector2.up;
        [SerializeField] private float _forceAmount = 5f;
        [SerializeField] private bool _effectsEnemies = false;

        private void OnTriggerStay2D(Collider2D other)
        {
            GameObject otherGameObject = other.gameObject;
            if (otherGameObject.CompareTag("Player"))
            {
                otherGameObject.TryGetComponent(out PlayerController playerController);
                if (playerController)
                {
                    Vector2 forceVector = _forceDirection * _forceAmount;

                    ref Vector2 playerVelocity = ref playerController.PlayerMovement.Velocity;

                    // Rightwards force
                    if (forceVector.x > 0f)
                    {
                        if (playerVelocity.x < forceVector.x)
                        {
                            playerVelocity.x = forceVector.x;
                        }
                    }
                    // Leftwards force
                    else if (forceVector.x < 0f)
                    {
                        if (playerVelocity.x > forceVector.x)
                        {
                            playerVelocity.x = forceVector.x;
                        }
                    }
                    
                    // Upwards force
                    if (forceVector.y > 0f)
                    {
                        if (playerVelocity.y < forceVector.y)
                        {
                            playerVelocity.y = forceVector.y;
                        }
                    }
                    // Downwards force
                    else if (forceVector.y < 0f)
                    {
                        if (playerVelocity.y > forceVector.y)
                        {
                            playerVelocity.y = forceVector.y;
                        }
                    }
                }
            }
            else if (_effectsEnemies && otherGameObject.CompareTag("Enemy"))
            {
                otherGameObject.TryGetComponent(out MovementController movementController);
                if (movementController)
                {
                    movementController.Move(_forceDirection * _forceAmount);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            GameObject otherGameObject = other.gameObject;
            if (otherGameObject.CompareTag("Player"))
            {
                otherGameObject.TryGetComponent(out PlayerController playerController);
                if (playerController)
                {
                    Vector2 forceVector = _forceDirection * _forceAmount;
                    // Sideways force
                    if (forceVector.x != 0f)
                    {
                        playerController.PlayerMovement.Velocity.x = 0;
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 position = transform.position;
            Gizmos.DrawLine(position, position + (_forceDirection * _forceAmount));
        }
    }
}