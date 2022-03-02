using System;
using Bolt;
using UnityEngine;

namespace Level
{
    public class HubArea : MonoBehaviour, ILevelArea
    {
        public Transform SpawnPoint;
        public Gateway HubExit;
        
        public void Initialise()
        {
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}