using System.Collections.Generic;
using Enemy;
using Entity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    // Note: this could end up being renamed to something more appropriate like run manager
    // not sure what this will exactly do yet.
    public class LevelManager : Singleton<LevelManager>
    {
        // This potentially could be a scriptable object eventually? Or a scene
        [SerializeField] private HubArea _hubArea;

        [SerializeField] private List<RunArea> _allRunAreas;
        private RunArea _activeRunArea;
        [SerializeField] private int _overrideRunAreaSelectionIndex = -1;
        
        [SerializeField] private BossArea _bossArea;

        // This might be a bit much for the level to control the player, but fine for now.
        [SerializeField] private GameObject _player;

        private void OnEnable()
        {
            ActivateLevel();
            
            ConnectAreas();

            SpawnPlayer();

            _player.TryGetComponent(out EntityHealth playerHealth);
            if (playerHealth)
            {
                playerHealth.OnEntityDead += ResetLevel;
            }
            else
            {
                Debug.LogError("No entity health on player?", this);
            }
        }

        private void OnDisable()
        {
            _player.TryGetComponent(out EntityHealth playerHealth);
            if (playerHealth)
            {
                playerHealth.OnEntityDead -= ResetLevel;
            }
            else
            {
                Debug.LogError("No entity health on player?", this);
            }
        }

        private void ResetLevel()
        {
            EnemyManager.Instance.ResetAllEnemies();
        }
        
        private void ActivateLevel()
        {
            // Hub
            _hubArea.Activate();
            
            // Disable all
            _allRunAreas.ForEach(level => level.Deactivate());
            if (_overrideRunAreaSelectionIndex == -1)
            {
                ActivateRandomLevel();
            }
            else
            {
                _activeRunArea = _allRunAreas[_overrideRunAreaSelectionIndex];
                _activeRunArea.Activate();
            }
            
            _bossArea.Activate();
        }

        // This could be called anywhere, eg on button press, when the player exits the hub, or whatever
        private void ActivateRandomLevel()
        {
            // Get random level, set that level to active
            _activeRunArea = _allRunAreas[Random.Range(0, _allRunAreas.Count)];
            _activeRunArea.Activate();
        }

        private void ConnectAreas()
        {
            // Connect hub exit to run entrance
            _hubArea.HubExit.SetExitPosition(_activeRunArea.Entrance);
            
            //Connect all exits to the hub boss entrance
            //AK 7/3/22 Could easily make each gate have its own location in future
            //this is just to test a commit
            _activeRunArea.RunExits.ForEach(p => p.SetExitPosition(_bossArea.Entrance));
        }

        private void SpawnPlayer()
        {
            _player.transform.position = _hubArea.SpawnPoint.position;
        }
    }
}