using UnityEngine;

namespace ButterflyHouse.Butterflies
{
    /// <summary>
    /// Represents a chrysalis spawn point that periodically generates butterflies.
    /// Includes visual pulsing based on energy state.
    /// </summary>
    public class Chrysalis : MonoBehaviour
    {
        [Header("Archetype")]
        [SerializeField] private ButterflyArchetype archetype;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 20f;
        [SerializeField] private bool spawnOnStart = false;
        [SerializeField] private float initialDelay = 0f;
        
        [Header("Visual")]
        [SerializeField] private Renderer chrysalisRenderer;
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseAmplitude = 0.2f;
        
        private float _timer;
        private float _energy;
        private bool _hasSpawnedFirst;
        private MaterialPropertyBlock _mpb;
        
        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            
            if (chrysalisRenderer == null)
            {
                chrysalisRenderer = GetComponent<Renderer>();
            }
        }
        
        private void Start()
        {
            _timer = -initialDelay;
            
            if (spawnOnStart && archetype != null)
            {
                SpawnButterfly();
                _hasSpawnedFirst = true;
            }
        }
        
        private void Update()
        {
            if (archetype == null || ButterflyManager.Instance == null)
                return;
            
            if (!ButterflyManager.Instance.CanSpawn)
                return;
            
            _timer += Time.deltaTime;
            
            // Update energy (0 to 1 based on spawn timer)
            _energy = Mathf.Clamp01(_timer / spawnInterval);
            
            // Update visual pulse
            UpdateVisuals();
            
            // Spawn when ready
            if (_timer >= spawnInterval)
            {
                SpawnButterfly();
                _timer = 0f;
            }
        }
        
        private void UpdateVisuals()
        {
            if (chrysalisRenderer == null) return;
            
            // Pulse effect based on energy
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude * _energy;
            
            chrysalisRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_PulseIntensity", _energy);
            _mpb.SetFloat("_PulseScale", pulse);
            chrysalisRenderer.SetPropertyBlock(_mpb);
        }
        
        /// <summary>
        /// Spawn a butterfly from this chrysalis.
        /// </summary>
        public void SpawnButterfly()
        {
            if (archetype == null)
            {
                Debug.LogWarning($"Chrysalis at {transform.position} has no archetype assigned.");
                return;
            }
            
            Vector3 spawnPosition = transform.position;
            // Add slight random offset
            spawnPosition += Random.insideUnitSphere * 0.2f;
            
            ButterflyManager.Instance?.SpawnButterfly(archetype, spawnPosition);
        }
        
        /// <summary>
        /// Set the archetype for this chrysalis.
        /// </summary>
        public void SetArchetype(ButterflyArchetype newArchetype)
        {
            archetype = newArchetype;
        }
        
        public ButterflyArchetype Archetype => archetype;
        public float Energy => _energy;
    }
}

