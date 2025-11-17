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
        [SerializeField] private float baseSpawnInterval = 20f;
        [SerializeField] private float spawnInterval = 20f;
        [SerializeField] private bool spawnOnStart = false;
        [SerializeField] private float initialDelay = 0f;
        
        [Header("Population Maintenance")]
        [SerializeField] private bool adjustSpawnRateForPopulation = true;
        [SerializeField] private float minSpawnInterval = 5f; // Fastest spawn rate when population is low
        [SerializeField] private float maxSpawnInterval = 30f; // Slowest spawn rate when population is high
        [SerializeField] private float spawnRateAdjustmentSpeed = 2f; // How quickly spawn rate adjusts
        
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
            spawnInterval = baseSpawnInterval;
            
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
            
            // Update spawn interval based on population maintenance
            if (adjustSpawnRateForPopulation)
            {
                UpdateSpawnInterval();
            }
            
            // Check if we can spawn (if at max, wait)
            if (!ButterflyManager.Instance.CanSpawn)
            {
                // Still update timer, but don't spawn
                // This allows chrysalises to be "ready" when space opens up
                return;
            }
            
            _timer += Time.deltaTime;
            
            // Update energy (0 to 1 based on spawn timer)
            _energy = Mathf.Clamp01(_timer / spawnInterval);
            
            // Update visual pulse
            UpdateVisuals();
            
            // Spawn when ready (or if population maintenance is needed)
            bool shouldSpawn = _timer >= spawnInterval;
            if (!shouldSpawn && ButterflyManager.Instance.ShouldSpawnForMaintenance)
            {
                // If population is below target, spawn even if timer isn't fully ready
                // This helps maintain steady population
                float minTimeForMaintenance = spawnInterval * 0.5f; // Spawn at 50% of interval if maintenance needed
                shouldSpawn = _timer >= minTimeForMaintenance;
            }
            
            if (shouldSpawn)
            {
                SpawnButterfly();
                _timer = 0f;
            }
        }
        
        /// <summary>
        /// Adjust spawn interval based on current population vs target.
        /// Spawns faster when population is below target, slower when at or above target.
        /// </summary>
        private void UpdateSpawnInterval()
        {
            if (ButterflyManager.Instance == null) return;
            
            float targetInterval = baseSpawnInterval;
            
            if (ButterflyManager.Instance.ShouldSpawnForMaintenance)
            {
                // Population is below target - spawn faster
                targetInterval = minSpawnInterval;
            }
            else
            {
                // Population is at or above target - spawn at normal or slower rate
                // Calculate based on how full the population is
                int currentCount = ButterflyManager.Instance.ActiveButterflyCount;
                int maxCount = ButterflyManager.Instance.CurrentMaxButterflies;
                
                if (maxCount > 0)
                {
                    float populationPercent = (float)currentCount / maxCount;
                    // When at 100% capacity, use max interval; when at 70% (min target), use base interval
                    float t = Mathf.InverseLerp(0.7f, 1f, populationPercent);
                    targetInterval = Mathf.Lerp(baseSpawnInterval, maxSpawnInterval, t);
                }
                else
                {
                    targetInterval = baseSpawnInterval;
                }
            }
            
            // Smoothly adjust spawn interval
            spawnInterval = Mathf.Lerp(spawnInterval, targetInterval, Time.deltaTime * spawnRateAdjustmentSpeed);
            spawnInterval = Mathf.Clamp(spawnInterval, minSpawnInterval, maxSpawnInterval);
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

