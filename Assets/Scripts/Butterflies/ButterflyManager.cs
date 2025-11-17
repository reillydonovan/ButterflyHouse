using System.Collections.Generic;
using UnityEngine;

namespace ButterflyHouse.Butterflies
{
    /// <summary>
    /// Central manager for all butterflies in the scene.
    /// Handles spawning, pooling, and lifecycle management.
    /// </summary>
    public class ButterflyManager : MonoBehaviour
    {
        public static ButterflyManager Instance { get; private set; }
        
        [Header("Prefabs")]
        [SerializeField] private Butterfly butterflyPrefab;
        
        [Header("Spawn Settings")]
        [SerializeField] private int maxButterflies = 20;
        [SerializeField] private bool enableAutoCleanup = true;
        [SerializeField] private float cleanupInterval = 5f;
        
        private readonly List<Butterfly> _activeButterflies = new List<Butterfly>();
        private float _cleanupTimer;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple ButterflyManager instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        private void Update()
        {
            if (enableAutoCleanup)
            {
                _cleanupTimer += Time.deltaTime;
                if (_cleanupTimer >= cleanupInterval)
                {
                    CleanupDissipatedButterflies();
                    _cleanupTimer = 0f;
                }
            }
        }
        
        /// <summary>
        /// Spawn a butterfly from the given archetype at the specified position.
        /// </summary>
        public Butterfly SpawnButterfly(ButterflyArchetype archetype, Vector3 position)
        {
            if (archetype == null)
            {
                Debug.LogError("Cannot spawn butterfly: archetype is null");
                return null;
            }
            
            if (_activeButterflies.Count >= maxButterflies)
            {
                Debug.Log($"Max butterflies ({maxButterflies}) reached. Cannot spawn new butterfly.");
                return null;
            }
            
            if (butterflyPrefab == null)
            {
                Debug.LogError("Cannot spawn butterfly: prefab is not assigned");
                return null;
            }
            
            var butterfly = Instantiate(butterflyPrefab, position, Quaternion.identity);
            butterfly.Initialize(archetype);
            _activeButterflies.Add(butterfly);
            
            return butterfly;
        }
        
        /// <summary>
        /// Remove a butterfly from the active list (called when butterfly is being destroyed).
        /// </summary>
        public void DespawnButterfly(Butterfly butterfly)
        {
            if (butterfly != null)
            {
                _activeButterflies.Remove(butterfly);
            }
        }
        
        /// <summary>
        /// Get all currently active butterflies.
        /// </summary>
        public List<Butterfly> GetActiveButterflies()
        {
            return new List<Butterfly>(_activeButterflies);
        }
        
        /// <summary>
        /// Get the count of active butterflies.
        /// </summary>
        public int ActiveButterflyCount => _activeButterflies.Count;
        
        /// <summary>
        /// Check if we can spawn more butterflies.
        /// </summary>
        public bool CanSpawn => _activeButterflies.Count < maxButterflies;
        
        /// <summary>
        /// Remove all butterflies from the scene.
        /// </summary>
        public void ClearAllButterflies()
        {
            foreach (var butterfly in _activeButterflies)
            {
                if (butterfly != null)
                {
                    butterfly.ForceDissipate();
                }
            }
            _activeButterflies.Clear();
        }
        
        private void CleanupDissipatedButterflies()
        {
            _activeButterflies.RemoveAll(b => b == null || b.IsDissipated);
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

