using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RL_HealthUI : MonoBehaviour
{
    [SerializeField] private IntVariable playerHealth;
    [SerializeField] private Text displayText;
    private int healthHolder;

    // Update is called once per frame
    void Update()
    {
        if(healthHolder != playerHealth.RuntimeValue)
        {
            ChangeHealthText();
        }
    }

    void ChangeHealthText()
    {
        displayText.text = (playerHealth.RuntimeValue.ToString() + " / " + playerHealth.InitialValue.ToString());
        healthHolder = playerHealth.RuntimeValue;
    }
}
