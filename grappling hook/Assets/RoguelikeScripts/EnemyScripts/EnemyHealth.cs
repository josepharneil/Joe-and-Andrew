using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 10;
    [SerializeField] private int currentHealth = 10;

    [SerializeField] private Text displayText;

    private Vector3 initialPosition;

    public enum EnemyState
    {
        Alive,
        Dead,
        Destroy
    }
    public EnemyState enemyState = EnemyState.Alive;

    private void Awake()
    {
        currentHealth = startingHealth;
        initialPosition = transform.position;

        displayText.text = "EnemyHealth: " + currentHealth.ToString() + "/" + startingHealth.ToString();
    }

    public void Heal( int value )
    {
        currentHealth += value;
        if( currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        displayText.text = "EnemyHealth: " + currentHealth.ToString() + "/" + startingHealth.ToString();
    }

    public void Damage( int value )
    {
        currentHealth -= value;
        if( currentHealth < 0)
        {
            currentHealth = 0;
        }
        displayText.text = "EnemyHealth: " + currentHealth.ToString() + "/" + startingHealth.ToString();

        if ( currentHealth == 0 )
        {
            MakeDead();
        }
    }

    private void MakeDead()
    {
        enemyState = EnemyState.Dead;

        StartCoroutine(WaitThenSetAlive());
    }

    private IEnumerator WaitThenSetAlive()
    {
        transform.position = initialPosition;
        yield return new WaitForSeconds(1);
        SetAlive();
    }

    private void SetAlive()
    {
        currentHealth = startingHealth;
        enemyState = EnemyState.Alive;
        displayText.text = "EnemyHealth: " + currentHealth.ToString() + "/" + startingHealth.ToString();
    }
}
