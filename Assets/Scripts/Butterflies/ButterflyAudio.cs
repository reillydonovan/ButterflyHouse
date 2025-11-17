using System.Collections;
using UnityEngine;
using ButterflyHouse.Core;

namespace ButterflyHouse.Butterflies
{
    /// <summary>
    /// Handles audio for individual butterflies.
    /// Manages pitch, volume, and frequency based on butterfly state and movement.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class ButterflyAudio : MonoBehaviour
    {
        [Header("Audio Source")]
        [SerializeField] private AudioSource audioSource;
        
        [Header("Modulation")]
        [Range(0f, 1f)]
        [SerializeField] private float speedToVolumeFactor = 0.5f;
        [Range(0f, 0.2f)]
        [SerializeField] private float pitchVariation = 0.1f;
        [Range(0f, 10f)]
        [SerializeField] private float lfoRate = 2f;
        
        private Butterfly _butterfly;
        private ButterflyArchetype _archetype;
        private float _basePitch = 1f;
        private float _baseVolume = 0.6f;
        private float _currentIntensity = 1f;
        private float _targetIntensity = 1f;
        private float _intensityVelocity;
        
        // Frequency tracking (for waveform sync)
        private static readonly System.Collections.Generic.Dictionary<Butterfly, ButterflyAudio> _activeAudios = 
            new System.Collections.Generic.Dictionary<Butterfly, ButterflyAudio>();
        
        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; // 3D sound
        }
        
        /// <summary>
        /// Initialize audio with butterfly and archetype.
        /// </summary>
        public void Initialize(Butterfly butterfly, ButterflyArchetype archetype)
        {
            _butterfly = butterfly;
            _archetype = archetype;
            
            if (archetype == null)
            {
                Debug.LogError("Cannot initialize ButterflyAudio: archetype is null");
                return;
            }
            
            _basePitch = archetype.basePitch;
            _baseVolume = archetype.audioVolume;
            
            if (archetype.baseTone != null)
            {
                audioSource.clip = archetype.baseTone;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"ButterflyAudio: Archetype '{archetype.name}' has no baseTone assigned.");
            }
            
            // Register for frequency queries
            if (_butterfly != null)
            {
                _activeAudios[_butterfly] = this;
            }
            
            // Apply settings
            if (Settings.Instance != null)
            {
                UpdateVolume(Settings.Instance.butterflyVolume);
            }
        }
        
        private void Update()
        {
            if (_butterfly == null || _archetype == null) return;
            
            // Smooth intensity changes
            _currentIntensity = Mathf.SmoothDamp(_currentIntensity, _targetIntensity, ref _intensityVelocity, 0.2f);
            
            // Modulate volume based on speed
            float speed = _butterfly.CurrentSpeed;
            float speedVolume = Mathf.Clamp01(speed * speedToVolumeFactor);
            float finalVolume = _baseVolume * _currentIntensity * speedVolume;
            
            if (Settings.Instance != null)
            {
                finalVolume *= Settings.Instance.butterflyVolume;
            }
            
            audioSource.volume = finalVolume;
            
            // Modulate pitch with LFO and movement
            float lfo = Mathf.Sin(Time.time * lfoRate) * pitchVariation;
            float speedPitch = speed * 0.1f; // Slight pitch increase with speed
            audioSource.pitch = _basePitch + lfo + speedPitch;
            
            // Update 3D position
            if (audioSource.isActiveAndEnabled && audioSource.clip != null)
            {
                audioSource.transform.position = _butterfly.transform.position;
            }
        }
        
        /// <summary>
        /// Set the intensity multiplier (0-1). Used for landing state, etc.
        /// </summary>
        public void SetIntensity(float intensity)
        {
            _targetIntensity = Mathf.Clamp01(intensity);
        }
        
        /// <summary>
        /// Get current audio intensity.
        /// </summary>
        public float GetCurrentIntensity()
        {
            return _currentIntensity;
        }
        
        /// <summary>
        /// Get the approximate frequency (Hz) of the current pitch.
        /// </summary>
        public float GetCurrentFrequency()
        {
            // Standard frequency calculation: 440Hz (A4) * pitch multiplier
            return 440f * audioSource.pitch;
        }
        
        /// <summary>
        /// Static method to get frequency for a butterfly (used by Butterfly.UpdateFlying).
        /// </summary>
        public static float GetCurrentFrequency(Butterfly butterfly)
        {
            if (butterfly == null) return 440f;
            
            if (_activeAudios.TryGetValue(butterfly, out var audio))
            {
                return audio.GetCurrentFrequency();
            }
            
            return 440f; // Default frequency
        }
        
        /// <summary>
        /// Update volume from settings.
        /// </summary>
        public void UpdateVolume(float volumeMultiplier)
        {
            // Volume is applied in Update()
        }
        
        /// <summary>
        /// Fade out audio smoothly.
        /// </summary>
        public IEnumerator FadeOut(float duration)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
            
            audioSource.volume = 0f;
            audioSource.Stop();
        }
        
        private void OnDestroy()
        {
            if (_butterfly != null)
            {
                _activeAudios.Remove(_butterfly);
            }
        }
    }
}

