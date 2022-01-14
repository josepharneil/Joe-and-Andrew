using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    // Maybe this class should be a base class for other to inherit from?
    public class CheckLookForPlayer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float closenessThreshold = 2f;

        public bool Check()
        {
            return Vector2.Distance(player.position, transform.position) < closenessThreshold;
        }

    }
}
