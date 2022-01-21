using UnityEngine;

/// This file is copied from Tarodev:
/// https://www.youtube.com/watch?v=tE1qH8OxO2Y

/// <summary>
/// A static instance is similar to a singleton, but instead of destroying any new
/// instances, it overrides the current instance. THis is handy for resetting the state
/// and saves you manually doing it.
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// This transforms the static instance into a basic Singleton. This will destroy any new
/// versions created, leaving the original instance intact.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        base.Awake();
    }
}

/// <summary>
/// Finally, we have a persistent version of the Singleton. This will survive through scene
/// loads. Perfect for system classes which require stateful, persistent data. Or audio
/// sources, where music plays through loading screens, etc.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}