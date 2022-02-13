using System.Collections.Generic;
using UnityEngine;
using Entity;

public class DebugShowEntityHealth : MonoBehaviour
{
    private readonly List<EntityHealth> _entityHealths = new List<EntityHealth>();
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        
        foreach( EntityHealth entityHealth in FindObjectsOfType<EntityHealth>() )
        {
            _entityHealths.Add(entityHealth);
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
}
