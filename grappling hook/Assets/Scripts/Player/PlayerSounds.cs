using System;
using Audio;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSounds", menuName = "PlayerSounds")]
    public class PlayerSounds : ScriptableObject
    {
        [SerializeField] private AudioClip _jumpSound;

        [Header("Debug")]
        [SerializeField] private bool _debugUseSounds = false;
        
        public void Initialise()
        {
            Debug.Assert(AudioManager.Instance != null, "No audio manager", this);
        }

        public void PlayJumpSound()
        {
            if (!_debugUseSounds) return;
            if (!AudioManager.Instance) return;
            AudioManager.Instance.PlaySound(_jumpSound);
        }
        
        public void PlayWallJumpSound()
        {
            if (!_debugUseSounds) return;
            if (!AudioManager.Instance) return;
            // Currently plays jump sound
            AudioManager.Instance.PlaySound(_jumpSound);
        }
    }
}