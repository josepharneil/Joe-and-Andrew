using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class SandbagEnemy : MonoBehaviour
{
    [SerializeField] private int health;
    public void DamageThisEnemy( int damage, Vector3 hitDirection, float knockbackStrength )
    {
        health -= damage;
        print("Hit! " + health);

        hitDirection.Normalize();
        hitDirection.y = 0f;
        hitDirection.z = 0f;
        transform.DOMove(transform.position + (hitDirection*knockbackStrength ), 1f).SetEase(Ease.OutCubic);
    }

    private void OnGUI()
    {
        GUILayout.TextArea("Text here!" + health.ToString());
    }
}
