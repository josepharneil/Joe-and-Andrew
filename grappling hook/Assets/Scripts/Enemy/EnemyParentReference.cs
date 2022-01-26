using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyParentReference : MonoBehaviour
    {
        public GameObject parent;
        private void OnValidate()
        {
            Debug.Assert(parent != null, "The parent gameobject for this enemy must not be null for destroying to work.", this);
        }
    }
}