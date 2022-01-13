using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RL_CollectableManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int collectablesLeft;
    private int initialCount;
    private int collectablesAquired;

    [SerializeField] private Text displayCountText;

    private static RL_CollectableManager _instance;
    public static RL_CollectableManager Instance { get { return _instance; } }

    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        initialCount = collectablesLeft = GameObject.FindGameObjectsWithTag("Collectable").Length;
        collectablesAquired = 0;
        
        displayCountText.text = (collectablesAquired.ToString() +"/" + initialCount.ToString());
    }

    public void CollectCollectable()
    {
        collectablesLeft -= 1;
        collectablesAquired += 1;
        displayCountText.text = (collectablesAquired.ToString()+"/" + initialCount.ToString()); 
    }
    


}
