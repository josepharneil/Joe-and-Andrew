using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class RunArea : MonoBehaviour, ILevelArea
    {
        // This could eventually be a list of entrances
        public Transform Entrance;
        //List of exits
        public List<Gateway> RunExits;

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