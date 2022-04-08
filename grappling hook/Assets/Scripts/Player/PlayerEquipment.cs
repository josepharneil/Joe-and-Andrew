using UnityEngine;

namespace Player
{
    /// <summary>
    /// This is what the player is currently holding / equipped with, including weapons, armour etc.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerEquipment", menuName = "PlayerEquipment", order = 0)]
    public class PlayerEquipment : ScriptableObject
    {
        // This eventually could be more generic? Could even be "right hand" and "left hand"
        // where you could do: RightHand.IsWeapon() ? RightHand.GetWeapon() : ... blah blah
        public MeleeWeapon CurrentMeleeWeapon;
    }
}