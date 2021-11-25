using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RL_HealthUI : MonoBehaviour
{
    [SerializeField] private IntVariable playerHealth;
    [SerializeField] private Text displayText;
    private int healthHolder;
    private Color red = new Color(255, 0, 0);
    private Color green = new Color(0, 255, 0);
    private Color orange = new Color(255, 165, 0);

    // Update is called once per frame
    void Update()
    {
        if(healthHolder != playerHealth.RuntimeValue)
        {
            
            ChangeHealthText();
            ChangeHealthColour();
        }
    }

    void ChangeHealthText()
    {
        displayText.text = (playerHealth.RuntimeValue.ToString() + " / " + playerHealth.InitialValue.ToString());
        healthHolder = playerHealth.RuntimeValue;
        


    }

    //AK: Changing colour by just doing color = new Color(r,g,b) doesn't work for some reason
    // only seems to work by doing already instantiated values
    void ChangeHealthColour()
    {
        if (playerHealth.RuntimeValue >= playerHealth.InitialValue * 2 / 3)
        {
            displayText.color = green;
        }
        else if (playerHealth.RuntimeValue >= playerHealth.InitialValue / 2)
        {
            displayText.color = orange;
        }
        else
        {
            displayText.color = red;
        }
    }
}
