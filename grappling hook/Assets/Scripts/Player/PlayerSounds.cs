using Audio;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSounds", menuName = "PlayerSounds")]
    public class PlayerSounds : ScriptableObject
    {
        [SerializeField] private AudioClip _jumpSound;
        
        private void Awake()
        {
            Debug.Assert(AudioManager.Instance != null, "No audio manager", this);
        }

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