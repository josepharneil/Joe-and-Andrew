using UnityEngine;

namespace Audio
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        // Optional: could add voices source
        [SerializeField] private AudioSource _musicSource, _effectsSource;

        [SerializeField] [Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _musicVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _effectsVolume = 1f;
        
        private void OnValidate()
        {
            ChangeMasterVolume(_masterVolume);
            ChangeEffectsVolume(_effectsVolume);
            ChangeMusicVolume(_musicVolume);
        }

        public void PlaySound(AudioClip clip)
        {
            _effectsSource.PlayOneShot(clip);
        }

        public void ChangeMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        public void ChangeMusicVolume(float value)
        {
            _musicSource.volume = value;
        }
        
        public void ChangeEffectsVolume(float value)
        {
            _effectsSource.volume = value;
        }

        public void ToggleEffects()
        {
            _effectsSource.mute = !_effectsSource.mute;
        }

        public void ToggleMusic()
        {
            _musicSource.mute = !_musicSource.mute;
        }
    }
}