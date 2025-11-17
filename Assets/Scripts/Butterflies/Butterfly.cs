using System.Collections;
using UnityEngine;
using ButterflyHouse.Core;
using ButterflyHouse.Interaction;

namespace ButterflyHouse.Butterflies
{
    /// <summary>
    /// Main butterfly behavior controller.
    /// Manages lifecycle states: Emerging, Flying, Landing, Dissipating.
    /// Handles procedural flight paths and interaction with landing targets.
    /// </summary>
    public class Butterfly : MonoBehaviour
    {
        public enum State
        {
            Emerging,
            Flying,
            Landing,
            Dissipating
        }
        
        [Header("Components")]
        [SerializeField] private ButterflyVisualController visualController;
        [SerializeField] private ButterflyAudio audioController;
        [SerializeField] private ButterflyFormEvolution formEvolution;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Collider butterflyCollider;
        
        [Header("Emerging")]
        [SerializeField] private float emergingDuration = 2f;
        [SerializeField] private AnimationCurve emergingScaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [Header("Landing")]
        [SerializeField] private float landingCheckInterval = 2f;
        [SerializeField] private float landingRadius = 3f;
        [SerializeField] private LayerMask landingTargetLayer = -1;
        [Range(1f, 30f)]
        [SerializeField] private float minLandingDuration = 2f;
        [Range(1f, 30f)]
        [SerializeField] private float maxLandingDuration = 8f;
        [Range(5f, 60f)]
        [SerializeField] private float minLandingCooldown = 10f;
        [Range(5f, 60f)]
        [SerializeField] private float maxLandingCooldown = 30f;
        
        private ButterflyArchetype _archetype;
        private State _currentState = State.Emerging;
        private Vector3 _velocity;
        private float _age;
        private float _landingTimer;
        private float _landingDuration;
        private float _landingStartTime;
        private float _landingCooldownEndTime;
        private Interaction.LandingTarget _currentLandingTarget;
        private Interaction.LandingTarget _lastLandingTarget;
        private Vector3 _landingOffset;
        private float _normalizedAge => _age / (_archetype?.lifetime ?? 60f);
        
        // Flight parameters
        private Vector3 _noiseOffset;
        private Vector3 _focalPoint;
        
        public State CurrentState => _currentState;
        public ButterflyArchetype Archetype => _archetype;
        public float CurrentSpeed => _velocity.magnitude;
        public bool IsDissipated => _currentState == State.Dissipating && !gameObject.activeSelf;
        public Vector3 Velocity => _velocity;
        
        private void Awake()
        {
            if (visualController == null)
                visualController = GetComponent<ButterflyVisualController>();
            
            if (audioController == null)
                audioController = GetComponent<ButterflyAudio>();
            
            if (formEvolution == null)
                formEvolution = GetComponent<ButterflyFormEvolution>();
            
            if (trailRenderer == null)
                trailRenderer = GetComponentInChildren<TrailRenderer>();
            
            if (butterflyCollider == null)
                butterflyCollider = GetComponent<Collider>();
            
            // Fix trail material early if it's invalid
            if (trailRenderer != null)
            {
                FixTrailMaterialIfNeeded();
            }
            
            _noiseOffset = Random.insideUnitSphere * 100f;
            _focalPoint = transform.position;
        }
        
        /// <summary>
        /// Check and fix trail material if it's missing or using an invalid shader.
        /// </summary>
        private void FixTrailMaterialIfNeeded()
        {
            if (trailRenderer == null) return;
            
            bool needsFix = false;
            
            // Check if material is null
            if (trailRenderer.sharedMaterial == null)
            {
                needsFix = true;
            }
            // Check if material uses an error shader
            else if (trailRenderer.sharedMaterial != null)
            {
                string shaderName = trailRenderer.sharedMaterial.shader.name;
                if (shaderName.Contains("Error") || 
                    shaderName == "Hidden/InternalErrorShader" ||
                    shaderName == "Hidden/InternalErrorShader (UnityEngine.Shader)")
                {
                    needsFix = true;
                }
            }
            
            if (needsFix)
            {
                Material trailMat = CreateTrailMaterial();
                if (trailMat != null)
                {
                    trailRenderer.sharedMaterial = trailMat;
                    Debug.Log($"Butterfly: Fixed trail material with shader '{trailMat.shader.name}'");
                }
                else
                {
                    Debug.LogWarning("Butterfly: Could not create trail material. Trail will appear magenta.");
                }
            }
        }
        
        /// <summary>
        /// Initialize the butterfly with an archetype.
        /// </summary>
        public void Initialize(ButterflyArchetype archetype)
        {
            if (archetype == null)
            {
                Debug.LogError("Cannot initialize butterfly: archetype is null");
                Destroy(gameObject);
                return;
            }
            
            _archetype = archetype;
            _currentState = State.Emerging;
            _age = 0f;
            _landingTimer = 0f;
            
            // Initialize scale to 0 for emerging
            transform.localScale = Vector3.zero;
            
            // Initialize components
            if (visualController != null)
            {
                visualController.Initialize(archetype);
            }
            
            if (audioController != null)
            {
                audioController.Initialize(this, archetype);
            }
            
            if (trailRenderer != null)
            {
                // Ensure trail material is valid (should be fixed in Awake, but double-check)
                FixTrailMaterialIfNeeded();
                
                if (Settings.Instance != null)
                {
                    trailRenderer.enabled = Settings.Instance.enableTrails;
                }
                else
                {
                    trailRenderer.enabled = true;
                }
                
                // Set trail color from archetype gradient
                var color = archetype.wingColorGradient.Evaluate(0f);
                trailRenderer.startColor = color;
                trailRenderer.endColor = new Color(color.r, color.g, color.b, 0f);
            }
            
            // Set initial velocity
            _velocity = Random.insideUnitSphere.normalized * archetype.flightSpeedCurve.Evaluate(0f);
            
            StartCoroutine(LifecycleCoroutine());
        }
        
        private IEnumerator LifecycleCoroutine()
        {
            // Emerging phase
            yield return StartCoroutine(EmergingCoroutine());
            
            // Main flying loop
            _currentState = State.Flying;
            
            while (_currentState == State.Flying && _age < _archetype.lifetime)
            {
                yield return null;
            }
            
            // Check if we should dissipate or land
            if (_currentState == State.Flying)
            {
                _currentState = State.Dissipating;
                yield return StartCoroutine(DissipatingCoroutine());
            }
        }
        
        private IEnumerator EmergingCoroutine()
        {
            float elapsed = 0f;
            
            while (elapsed < emergingDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / emergingDuration;
                float scale = emergingScaleCurve.Evaluate(t) * _archetype.baseScale;
                transform.localScale = Vector3.one * scale;
                
                yield return null;
            }
            
            transform.localScale = Vector3.one * _archetype.baseScale;
        }
        
        private IEnumerator DissipatingCoroutine()
        {
            // Fade out visual
            if (visualController != null)
            {
                StartCoroutine(visualController.FadeOut(2f));
            }
            
            // Fade out audio
            if (audioController != null)
            {
                StartCoroutine(audioController.FadeOut(2f));
            }
            
            yield return new WaitForSeconds(2f);
            
            // Cleanup
            ButterflyManager.Instance?.DespawnButterfly(this);
            Destroy(gameObject);
        }
        
        private void Update()
        {
            if (_archetype == null) return;
            
            _age += Time.deltaTime;
            
            UpdateState();
            
            switch (_currentState)
            {
                case State.Emerging:
                    // Scale handled in coroutine
                    break;
                    
                case State.Flying:
                    UpdateFlying();
                    CheckForLandingTargets();
                    break;
                    
                case State.Landing:
                    UpdateLanding();
                    break;
                    
                case State.Dissipating:
                    // Handled in coroutine
                    break;
            }
            
            // Update visual parameters based on age
            UpdateVisualsFromAge();
        }
        
        private void UpdateState()
        {
            // Check if lifetime expired
            if (_age >= _archetype.lifetime && _currentState == State.Flying)
            {
                _currentState = State.Dissipating;
            }
        }
        
        private void UpdateFlying()
        {
            float t = Time.time;
            float speed = _archetype.flightSpeedCurve.Evaluate(_normalizedAge);
            
            // Perlin noise-based wandering
            Vector3 noiseDir = new Vector3(
                Mathf.PerlinNoise(t * _archetype.noiseScale + _noiseOffset.x, _noiseOffset.y) - 0.5f,
                Mathf.PerlinNoise(_noiseOffset.z, t * _archetype.noiseScale + _noiseOffset.x) - 0.5f,
                Mathf.PerlinNoise(t * _archetype.noiseScale + _noiseOffset.y, _noiseOffset.z) - 0.5f
            );
            
            // Audio-linked vertical oscillation
            float audioFrequency = ButterflyAudio.GetCurrentFrequency(this);
            float bob = Mathf.Sin(t * audioFrequency * 0.01f) * 0.01f;
            noiseDir.y += bob;
            
            // Constrain to flight radius
            Vector3 toFocal = transform.position - _focalPoint;
            float distance = toFocal.magnitude;
            if (distance > _archetype.maxFlightRadius)
            {
                Vector3 pullBack = -toFocal.normalized * (distance - _archetype.maxFlightRadius) * 0.5f;
                noiseDir += pullBack;
            }
            else if (distance < _archetype.minFlightRadius)
            {
                Vector3 pushOut = toFocal.normalized * (_archetype.minFlightRadius - distance) * 0.5f;
                noiseDir += pushOut;
            }
            
            // Update velocity
            Vector3 targetDir = noiseDir.normalized;
            _velocity = Vector3.Lerp(_velocity, targetDir * speed, Time.deltaTime * _archetype.turnSpeed);
            
            // Apply movement
            transform.position += _velocity * Time.deltaTime;
            
            // Update rotation to face movement direction
            if (_velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _archetype.turnSpeed);
            }
        }
        
        private void CheckForLandingTargets()
        {
            _landingTimer += Time.deltaTime;
            if (_landingTimer < landingCheckInterval) return;
            
            _landingTimer = 0f;
            
            // Only check if we're not already landing and it's time to land
            if (_currentLandingTarget != null) return;
            
            // Check if we're in landing cooldown period
            if (Time.time < _landingCooldownEndTime) return;
            
            // Random chance to seek landing target
            if (Random.value > 0.3f) return;
            
            Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, landingRadius, landingTargetLayer);
            
            // Prioritize fruits, then other targets
            Interaction.LandingTarget fruitTarget = null;
            Interaction.LandingTarget otherTarget = null;
            
            foreach (var target in nearbyTargets)
            {
                var landingTarget = target.GetComponent<Interaction.LandingTarget>();
                if (landingTarget == null || !landingTarget.IsAvailable) continue;
                
                // Skip if this is the last place we landed (unless we've landed somewhere else since)
                if (landingTarget == _lastLandingTarget) continue;
                
                // Check if it's a fruit
                if (landingTarget.Type == Interaction.LandingTarget.TargetType.Fruit)
                {
                    fruitTarget = landingTarget;
                    break; // Prioritize fruits, take first available
                }
                else if (otherTarget == null)
                {
                    otherTarget = landingTarget;
                }
            }
            
            // Prefer fruit over other targets
            Interaction.LandingTarget chosenTarget = fruitTarget ?? otherTarget;
            if (chosenTarget != null)
            {
                AttemptLanding(chosenTarget);
            }
        }
        
        private void AttemptLanding(Interaction.LandingTarget target)
        {
            _currentLandingTarget = target;
            _currentLandingTarget.Reserve(this);
            _currentState = State.Landing;
            
            // Set random landing duration
            _landingDuration = Random.Range(minLandingDuration, maxLandingDuration);
            _landingStartTime = Time.time;
            
            // Calculate landing offset
            Vector3 toButterfly = transform.position - target.transform.position;
            _landingOffset = target.transform.InverseTransformVector(toButterfly);
            _landingOffset = Vector3.ClampMagnitude(_landingOffset, 0.3f);
            
            // Notify fruit if it's a fruit target
            Plants.GenerativeFruit fruit = target.GetComponent<Plants.GenerativeFruit>();
            if (fruit == null && target.transform.parent != null)
            {
                fruit = target.transform.parent.GetComponent<Plants.GenerativeFruit>();
            }
            if (fruit != null)
            {
                fruit.OnButterflyLanded(this);
            }
        }
        
        private void UpdateLanding()
        {
            if (_currentLandingTarget == null)
            {
                _currentState = State.Flying;
                return;
            }
            
            Vector3 targetPos = _currentLandingTarget.transform.position + _currentLandingTarget.transform.TransformVector(_landingOffset);
            Vector3 toTarget = targetPos - transform.position;
            
            // Smoothly move to landing position
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
            
            // Orient towards landing target
            if (toTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
            
            // Check if landed (close enough)
            if (toTarget.sqrMagnitude < 0.01f)
            {
                // Reduce audio while landed
                if (audioController != null)
                {
                    audioController.SetIntensity(0.3f);
                }
                
                // Check if landing duration has elapsed
                float landingTimeElapsed = Time.time - _landingStartTime;
                if (landingTimeElapsed >= _landingDuration)
                {
                    // Landing duration complete, take off
                    TakeOff();
                }
            }
        }
        
        public void TakeOff()
        {
            Interaction.LandingTarget targetToRelease = _currentLandingTarget;
            
            if (_currentLandingTarget != null)
            {
                // Check if landing on player hand
                HandProxy handProxy = _currentLandingTarget.GetComponent<HandProxy>();
                if (handProxy == null && _currentLandingTarget.transform.parent != null)
                {
                    handProxy = _currentLandingTarget.transform.parent.GetComponent<HandProxy>();
                }
                
                if (handProxy != null)
                {
                    // Notify ecosystem manager that butterfly landed on player
                    if (Core.EcosystemStateController.Instance != null)
                    {
                        Core.EcosystemStateController.Instance.OnButterflyLandOnPlayer();
                    }
                }
                
                // Store this as the last landing target
                _lastLandingTarget = _currentLandingTarget;
                
                // Notify fruit if it's a fruit target
                Plants.GenerativeFruit fruit = _currentLandingTarget.GetComponent<Plants.GenerativeFruit>();
                if (fruit == null && _currentLandingTarget.transform.parent != null)
                {
                    fruit = _currentLandingTarget.transform.parent.GetComponent<Plants.GenerativeFruit>();
                }
                if (fruit != null)
                {
                    fruit.OnButterflyLeft(this);
                }
                
                // Check if landing on plant
                Plants.GenerativePlant plant = _currentLandingTarget.GetComponent<Plants.GenerativePlant>();
                if (plant == null && _currentLandingTarget.transform.parent != null)
                {
                    plant = _currentLandingTarget.transform.parent.GetComponent<Plants.GenerativePlant>();
                }
                if (plant != null)
                {
                    // Notify ecosystem manager of plant interaction
                    if (Core.EcosystemStateController.Instance != null)
                    {
                        Core.EcosystemStateController.Instance.OnButterflyPlantInteraction();
                    }
                    
                    // Notify plant growth system of butterfly visit
                    Plants.PlantGrowthSystem growthSystem = plant.GetComponent<Plants.PlantGrowthSystem>();
                    if (growthSystem == null && plant.transform.parent != null)
                    {
                        growthSystem = plant.transform.parent.GetComponent<Plants.PlantGrowthSystem>();
                    }
                    if (growthSystem != null)
                    {
                        growthSystem.OnButterflyVisit();
                    }
                }
                
                _currentLandingTarget.Release();
                _currentLandingTarget = null;
            }
            
            _currentState = State.Flying;
            
            // Set random landing cooldown - won't try to land again for this duration
            float cooldownDuration = Random.Range(minLandingCooldown, maxLandingCooldown);
            _landingCooldownEndTime = Time.time + cooldownDuration;
            
            // Reset audio
            if (audioController != null)
            {
                audioController.SetIntensity(1f);
            }
            
            // Give a little boost
            _velocity = Random.insideUnitSphere.normalized * _archetype.flightSpeedCurve.Evaluate(_normalizedAge);
        }
        
        private void UpdateVisualsFromAge()
        {
            if (visualController == null || _archetype == null) return;
            
            // Update color based on gradient
            Color currentColor = _archetype.wingColorGradient.Evaluate(_normalizedAge);
            visualController.SetColor(currentColor);
            
            // Update flap frequency
            float flapFreq = _archetype.flapFrequencyCurve.Evaluate(_normalizedAge);
            visualController.SetFlapFrequency(flapFreq);
            
            // Update wave parameters based on audio
            if (audioController != null)
            {
                float audioIntensity = audioController.GetCurrentIntensity();
                visualController.SetWaveParams(audioIntensity * 0.1f, audioIntensity * 5f);
                visualController.SetEmission(audioIntensity * 0.5f);
            }
        }
        
        public void ForceDissipate()
        {
            if (_currentState == State.Dissipating) return;
            
            if (_currentLandingTarget != null)
            {
                _currentLandingTarget.Release();
                _currentLandingTarget = null;
            }
            
            _currentState = State.Dissipating;
            StopAllCoroutines();
            StartCoroutine(DissipatingCoroutine());
        }
        
        /// <summary>
        /// Create a trail material with URP-compatible shader at runtime.
        /// </summary>
        private Material CreateTrailMaterial()
        {
            // Try URP shaders first
            Shader trailShader = Shader.Find("Universal Render Pipeline/Unlit") ??
                                 Shader.Find("Universal Render Pipeline/Simple Lit") ??
                                 Shader.Find("Unlit/Color") ??
                                 Shader.Find("Sprites/Default") ??
                                 Shader.Find("Legacy Shaders/Particles/Alpha Blended");
            
            if (trailShader == null)
            {
                Debug.LogWarning($"Butterfly: Could not find suitable trail shader. Trail may appear magenta.");
                return null;
            }
            
            Material mat = new Material(trailShader);
            mat.name = "TrailMaterial_Runtime";
            
            // Set color to white (trail colors come from vertex colors via startColor/endColor)
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", Color.white);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", Color.white);
            }
            
            // Enable vertex colors if supported
            if (mat.HasProperty("_VertexColorMode"))
            {
                mat.SetFloat("_VertexColorMode", 1f);
            }
            
            return mat;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_archetype == null) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_focalPoint, _archetype.minFlightRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_focalPoint, _archetype.maxFlightRadius);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, landingRadius);
        }
    }
}

