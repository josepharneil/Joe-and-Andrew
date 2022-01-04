using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
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
        print("Damage! " + maxHealth);
    }

    public void Heal( int heal )
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        print("Heal! " + maxHealth);
    }
}
