using System;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// A very light and simple class for health. Generally should be accessed via the EntityHitBox
    /// </summary>
    public class EntityHealth : MonoBehaviour
    {
        // Todo this eventually should be a scriptableobject
        [SerializeField] private int maxHealth = 100;
        public int CurrentHealth { get; private set; }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public event Action OnEntityDead;

        private void Start()
        {
            CurrentHealth = maxHealth;
        }

        public void Damage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnEntityDead?.Invoke();
            }
        }

        public void Heal(int heal)
        {
            CurrentHealth += heal;
            if (CurrentHealth > maxHealth)
            {
                CurrentHealth = maxHealth;
            }
        }

        public void HealToMax()
        {
            CurrentHealth = maxHealth;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }
    }
}
