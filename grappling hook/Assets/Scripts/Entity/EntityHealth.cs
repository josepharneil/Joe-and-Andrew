using UnityEngine;

namespace Entity
{
    /// <summary>
    /// A very light and simple class for health. Generally should be accessed via the EntityHitBox
    /// </summary>
    public class EntityHealth : MonoBehaviour
    {
        // Todo this eventually should be a scriptableobject
        public int maxHealth = 100;
        [HideInInspector] public int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void Damage( int damage )
        {
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
        }

        public void Heal( int heal )
        {
            currentHealth += heal;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public bool IsAlive()
        {
            return currentHealth > 0;
        }
    }
}
