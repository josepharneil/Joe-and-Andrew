using Audio;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSounds", menuName = "PlayerSounds")]
    public class PlayerSounds : ScriptableObject
    {
        [SerializeField] private AudioClip _jumpSound;

        public void PlayJumpSound()
        {
            AudioManager.Instance.PlaySound(_jumpSound);
        }
        
        public void PlayWallJumpSound()
        {
            // Currently plays jump sound
            AudioManager.Instance.PlaySound(_jumpSound);
        }
    }
}