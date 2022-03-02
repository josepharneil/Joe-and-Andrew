using UnityEngine;

namespace Level
{
    public class RunArea : MonoBehaviour, ILevelArea
    {
        // This could eventually be a list of entrances
        public Transform Entrance;

        public Gateway RunExit;

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