using Audio;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSounds", menuName = "PlayerSounds")]
    public class PlayerSounds : ScriptableObject
    {
        [SerializeField] private AudioClip _jumpSound;
        
        private void Start()
        {
            Debug.Assert(AudioManager.Instance != null, "No audio manager", this);
        }

        public void PlayJumpSound()
        {
            if (!AudioManager.Instance) return;
            AudioManager.Instance.PlaySound(_jumpSound);
        }
        
        public void PlayWallJumpSound()
        {
            if (!AudioManager.Instance) return;
            // Currently plays jump sound
            AudioManager.Instance.PlaySound(_jumpSound);
        }
    }
}