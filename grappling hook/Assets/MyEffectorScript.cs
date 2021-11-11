using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEffectorScript : MonoBehaviour
{

    public EffectorManager.EffectorType effectorType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.tag == "Player" )
        {
            EffectorManager.Instance.ApplyEffect(effectorType);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            EffectorManager.Instance.RemoveEffect(effectorType);
        }
    }
}
