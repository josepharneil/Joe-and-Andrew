using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 10;
    [SerializeField] private int currentHealth = 10;

    [SerializeField] private Text displayText;

    private void Awake()
    {
        currentHealth = startingHealth;
        UpdateHealthText();
    }

    public void HealToFull()
    {
        Heal(startingHealth - currentHealth);
    }

    public void Heal( int value )
    {
        currentHealth += value;
        if( currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        UpdateHealthText();
    }

    public void Damage( int value )
    {
        currentHealth -= value;
        if( currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        displayText.text = "EnemyHealth: " + currentHealth.ToString() + "/" + startingHealth.ToString();
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public bool IsDead()
    {
        return currentHealth == 0;
    }
}
