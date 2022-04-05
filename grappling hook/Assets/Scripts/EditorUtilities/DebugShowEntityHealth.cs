using System.Collections.Generic;
using Enemy;
using Entity;
using UnityEngine;

namespace EditorUtilities
{
    public class DebugShowEntityHealth : MonoBehaviour
    {
#if UNITY_EDITOR
        private readonly List<EntityHealth> _entityHealths = new List<EntityHealth>();
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            EnemyManager.OnEnemiesSpawnedOrDestroyed += SearchForAllEntityHealths;
        }

        private void OnDisable()
        {
            EnemyManager.OnEnemiesSpawnedOrDestroyed -= SearchForAllEntityHealths;
        }

        private void SearchForAllEntityHealths()
        {
            _entityHealths.Clear();
            foreach( EntityHealth entityHealth in FindObjectsOfType<EntityHealth>() )
            {
                if (entityHealth.gameObject.activeSelf)
                {
                    _entityHealths.Add(entityHealth);
                }
            }
        }
    
        private void OnGUI()
        {
            GUI.skin.box.fontSize = 24;
            foreach (EntityHealth entityHealth in _entityHealths)
            {
                if (entityHealth == null)
                {
                    _entityHealths.Remove(entityHealth);
                    return;
                }
                Vector2 targetPos = _mainCamera.WorldToScreenPoint(entityHealth.transform.position);
                targetPos.y += 1;
                GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 120, 40),
                    entityHealth.CurrentHealth + "/" + entityHealth.GetMaxHealth());
            }
        }
#endif
    }
}
