using UnityEngine;

namespace Level
{
    public class BossArea : MonoBehaviour, ILevelArea
    {
        public Transform Entrance;
        
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