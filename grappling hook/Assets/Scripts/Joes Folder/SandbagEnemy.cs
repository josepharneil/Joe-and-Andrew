using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class SandbagEnemy : MonoBehaviour
{
    [SerializeField] private int health;
    public void DamageThisEnemy( int damage )
    {
        health -= damage;
        print("Hit! "+health);

        transform.DOMove(transform.position + (Vector3)(Vector2.right*0.5f ), 1f).SetEase(Ease.OutElastic);
    }

    private void OnGUI()
    {
        GUILayout.TextArea("Text here!" + health.ToString());
    }
}
