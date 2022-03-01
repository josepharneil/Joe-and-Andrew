using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    // Note: this could end up being renamed to something more appropriate like run manager
    // not sure what this will exactly do yet.
    public class LevelManager : Singleton<LevelManager>
    {
        // This could end up being a list of "level" or something
        // This is the list of levels that we will select.
        [SerializeField] private List<GameObject> _levels;

        // Hub, could be a class
        [SerializeField] private GameObject _hub;
        
        // Boss / exit area, again, could be a class.
        [SerializeField] private GameObject _bossArea;

        private void OnEnable()
        {
            // Disable all
            _levels.ForEach(level => level.SetActive(false));

            ActivateRandomLevel();
        }

        // This could be called anywhere, eg on button press, when the player exits the hub, or whatever
        private void ActivateRandomLevel()
        {
            // Get random level

            // Set that level to active
        }

        private void LinkAreas()
        {
            // This would link together the hub, the run area, and the boss via teleportation zones.
            // Not sure how yet :D
        }
    }
}