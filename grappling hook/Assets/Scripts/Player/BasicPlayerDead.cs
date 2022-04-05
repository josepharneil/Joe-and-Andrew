using System;
using Entity;
using UnityEngine;

namespace Player
{
    public class BasicPlayerDead : MonoBehaviour
    {
        [SerializeField] private EntityHealth _entityHealth;
        private Vector2 _initialSpawnPosition;

        private void Start()
        {
            _initialSpawnPosition = transform.position;
        }

        private void OnEnable()
        {
            _entityHealth.OnEntityDead += PlayerDead;
        }

        private void OnDisable()
        {
            _entityHealth.OnEntityDead -= PlayerDead;
        }

        private void PlayerDead()
        {
            transform.position = _initialSpawnPosition;
            _entityHealth.HealToMax();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                _entityHealth.Damage(9999);
            }
        }
    }
}