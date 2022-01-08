using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PluggableAI
{
    /// <summary>
    /// Things the AI does that DO NOT have potential to change state, eg patrolling or attacking.
    /// Note that a C# class called Action already exists, so I'm calling this StateAction to avoid annoying conflicts.
    /// </summary>
    public abstract class StateAction : ScriptableObject
    {
        public abstract void Act(StateController controller);
    }
}
