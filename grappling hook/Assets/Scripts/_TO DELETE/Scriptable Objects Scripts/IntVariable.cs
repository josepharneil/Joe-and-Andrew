using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: I've removed this for now because we don't need it.
//[CreateAssetMenu]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public int InitialValue;

    [HideInInspector]
    public int RuntimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }

    public void OnBeforeSerialize() { }
}
