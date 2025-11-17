using UnityEngine;
using ButterflyHouse.Core;
using ButterflyHouse.Interaction;

namespace ButterflyHouse.Plants
{
    /// <summary>
    /// Interactive fruit that butterflies can land on and feed from.
    /// Provides a landing target and optional visual/audio feedback when butterflies land.
    /// </summary>
    public class GenerativeFruit : MonoBehaviour
    {
        [Header("Landing Target")]
        [SerializeField] private LandingTarget landingTarget;
        [SerializeField] private bool createLandingTarget = true;
        [Range(0.1f, 2f)]
        [SerializeField] private float landingZoneRadius = 0.3f;
        
        [Header("Visual")]
        [SerializeField] private Renderer fruitRenderer;
        [Range(0f, 1f)]
        [SerializeField] private float glowIntensity = 0.5f;
        [SerializeField] private bool animateGlow = true;
        [Range(0.5f, 3f)]
        [SerializeField] private float glowSpeed = 1f;
        
        [Header("Audio")]
        [SerializeField] private AudioClip[] landingClips;
        [SerializeField] private AudioSource audioSource;
        [Range(0f, 1f)]
        [SerializeField] private float audioVolume = 0.6f;
        [SerializeField] private bool playOnButterflyLand = true;
        
        [Header("Feeding")]
        [SerializeField] private bool canBeConsumed = false;
        [Range(0.5f, 10f)]
        [SerializeField] private float consumptionTime = 5f;
        [SerializeField] private GameObject consumableVisual;
        
        private MaterialPropertyBlock _mpb;
        private float _glowPhase = 0f;
        private float _currentGlow = 0f;
        private bool _isBeingConsumed = false;
        private float _consumptionTimer = 0f;
        
        private void Awake()
        {
            if (fruitRenderer == null)
                fruitRenderer = GetComponent<Renderer>();
            
            if (fruitRenderer != null)
            {
                _mpb = new MaterialPropertyBlock();
            }
            
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            
            // Set up audio source
            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                audioSource.spatialBlend = 1f; // 3D sound
            }
            
            // Create landing target if needed
            if (createLandingTarget && landingTarget == null)
            {
                CreateLandingTarget();
            }
        }
        
        private void Update()
        {
            // Animate glow if enabled
            if (animateGlow && fruitRenderer != null && fruitRenderer.sharedMaterial != null)
            {
                _glowPhase += Time.deltaTime * glowSpeed;
                _currentGlow = 0.5f + Mathf.Sin(_glowPhase) * glowIntensity * 0.5f;
                
                fruitRenderer.GetPropertyBlock(_mpb);
                if (fruitRenderer.sharedMaterial.HasProperty("_EmissionStrength"))
                    _mpb.SetFloat("_EmissionStrength", _currentGlow);
                else if (fruitRenderer.sharedMaterial.HasProperty("_Emission"))
                    _mpb.SetFloat("_Emission", _currentGlow);
                fruitRenderer.SetPropertyBlock(_mpb);
            }
            
            // Handle consumption
            if (_isBeingConsumed && canBeConsumed)
            {
                _consumptionTimer += Time.deltaTime;
                
                if (_consumptionTimer >= consumptionTime)
                {
                    OnConsumed();
                }
                else
                {
                    // Visual feedback during consumption
                    float consumptionProgress = _consumptionTimer / consumptionTime;
                    UpdateConsumptionVisual(consumptionProgress);
                }
            }
        }
        
        private void CreateLandingTarget()
        {
            GameObject landingObj = new GameObject("LandingTarget");
            landingObj.transform.SetParent(transform);
            landingObj.transform.localPosition = Vector3.zero;
            
            // Add sphere collider for landing detection
            SphereCollider collider = landingObj.AddComponent<SphereCollider>();
            collider.radius = landingZoneRadius;
            collider.isTrigger = true;
            
            // Add LandingTarget component
            landingTarget = landingObj.AddComponent<LandingTarget>();
            
            // Set target type to Fruit
            var field = typeof(LandingTarget).GetField("targetType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(landingTarget, LandingTarget.TargetType.Fruit);
            }
        }
        
        /// <summary>
        /// Called when a butterfly lands on this fruit.
        /// </summary>
        public void OnButterflyLanded(Butterflies.Butterfly butterfly)
        {
            if (butterfly == null) return;
            
            // Visual feedback
            PulseGlow();
            
            // Audio feedback
            if (playOnButterflyLand && audioSource != null && landingClips != null && landingClips.Length > 0)
            {
                AudioClip clip = landingClips[Random.Range(0, landingClips.Length)];
                if (clip != null)
                {
                    float volume = audioVolume;
                    if (Core.Settings.Instance != null)
                    {
                        volume *= Core.Settings.Instance.plantVolume;
                    }
                    audioSource.PlayOneShot(clip, volume);
                }
            }
            
            // Notify ecosystem manager of butterfly-plant interaction
            if (Core.EcosystemStateController.Instance != null)
            {
                Core.EcosystemStateController.Instance.OnButterflyPlantInteraction();
            }
            
            // Start consumption if enabled
            if (canBeConsumed && !_isBeingConsumed)
            {
                _isBeingConsumed = true;
                _consumptionTimer = 0f;
            }
        }
        
        /// <summary>
        /// Called when a butterfly leaves this fruit.
        /// </summary>
        public void OnButterflyLeft(Butterflies.Butterfly butterfly)
        {
            if (canBeConsumed && _isBeingConsumed)
            {
                _isBeingConsumed = false;
                _consumptionTimer = 0f;
                UpdateConsumptionVisual(0f);
            }
        }
        
        private void PulseGlow()
        {
            if (fruitRenderer == null) return;
            
            _currentGlow = 1f;
            fruitRenderer.GetPropertyBlock(_mpb);
            if (fruitRenderer.sharedMaterial != null)
            {
                if (fruitRenderer.sharedMaterial.HasProperty("_EmissionStrength"))
                    _mpb.SetFloat("_EmissionStrength", _currentGlow);
                else if (fruitRenderer.sharedMaterial.HasProperty("_Emission"))
                    _mpb.SetFloat("_Emission", _currentGlow);
            }
            fruitRenderer.SetPropertyBlock(_mpb);
            
            // Fade back
            StartCoroutine(FadeGlowCoroutine());
        }
        
        private System.Collections.IEnumerator FadeGlowCoroutine()
        {
            float startGlow = _currentGlow;
            float elapsed = 0f;
            float duration = 0.5f;
            
            while (elapsed < duration && fruitRenderer != null && fruitRenderer.sharedMaterial != null)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _currentGlow = Mathf.Lerp(startGlow, 0.5f + glowIntensity * 0.5f, t);
                
                fruitRenderer.GetPropertyBlock(_mpb);
                if (fruitRenderer.sharedMaterial.HasProperty("_EmissionStrength"))
                    _mpb.SetFloat("_EmissionStrength", _currentGlow);
                else if (fruitRenderer.sharedMaterial.HasProperty("_Emission"))
                    _mpb.SetFloat("_Emission", _currentGlow);
                fruitRenderer.SetPropertyBlock(_mpb);
                
                yield return null;
            }
        }
        
        private void UpdateConsumptionVisual(float progress)
        {
            if (consumableVisual != null)
            {
                // Scale down as consumed
                float scale = 1f - progress * 0.5f; // Reduce to 50% size
                consumableVisual.transform.localScale = Vector3.one * scale;
            }
            
            // Update material color/alpha
            if (fruitRenderer != null)
            {
                fruitRenderer.GetPropertyBlock(_mpb);
                Color baseColor = Color.white;
                if (fruitRenderer.sharedMaterial != null)
                {
                    if (fruitRenderer.sharedMaterial.HasProperty("_BaseColor"))
                        baseColor = _mpb.GetColor("_BaseColor");
                    else if (fruitRenderer.sharedMaterial.HasProperty("_Color"))
                        baseColor = _mpb.GetColor("_Color");
                    
                    baseColor.a = 1f - progress * 0.3f; // Fade slightly
                    
                    if (fruitRenderer.sharedMaterial.HasProperty("_BaseColor"))
                        _mpb.SetColor("_BaseColor", baseColor);
                    else if (fruitRenderer.sharedMaterial.HasProperty("_Color"))
                        _mpb.SetColor("_Color", baseColor);
                }
                
                fruitRenderer.SetPropertyBlock(_mpb);
            }
        }
        
        private void OnConsumed()
        {
            _isBeingConsumed = false;
            _consumptionTimer = 0f;
            
            // Disable or destroy the fruit
            if (consumableVisual != null)
            {
                consumableVisual.SetActive(false);
            }
            
            // Disable landing target
            if (landingTarget != null)
            {
                landingTarget.gameObject.SetActive(false);
            }
            
            // Disable glow
            _currentGlow = 0f;
            if (fruitRenderer != null)
            {
                fruitRenderer.GetPropertyBlock(_mpb);
                if (fruitRenderer.sharedMaterial != null)
                {
                    if (fruitRenderer.sharedMaterial.HasProperty("_EmissionStrength"))
                        _mpb.SetFloat("_EmissionStrength", 0f);
                    else if (fruitRenderer.sharedMaterial.HasProperty("_Emission"))
                        _mpb.SetFloat("_Emission", 0f);
                }
                fruitRenderer.SetPropertyBlock(_mpb);
            }
        }
        
        public LandingTarget LandingTarget => landingTarget;
        public bool IsAvailable => landingTarget != null && landingTarget.IsAvailable;
    }
}

