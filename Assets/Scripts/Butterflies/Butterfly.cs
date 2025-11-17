using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ButterflyHouse.Core;
using ButterflyHouse.Interaction;
using ButterflyHouse.Flowers;

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
        [SerializeField] private ButterflyEnergy energySystem;
        [SerializeField] private ButterflyPollination pollinationSystem;
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
        
        [Header("Flocking")]
        [SerializeField] private bool enableFlocking = true;
        [SerializeField] private float flockDetectionRadius = 5f;
        [SerializeField] private float flockCohesionWeight = 1f;
        [SerializeField] private float flockAlignmentWeight = 1f;
        [SerializeField] private float flockSeparationWeight = 1.5f;
        [SerializeField] private float flockSeparationDistance = 2f;
        [SerializeField] private float flockBlendSpeed = 2f;
        [Range(0f, 1f)]
        [SerializeField] private float breakOutChance = 0.1f;
        [SerializeField] private float breakOutCheckInterval = 3f;
        [SerializeField] private float maxFlockTime = 30f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true; // Enabled by default for debugging
        [SerializeField] private float debugLogInterval = 2f;
        
        [Header("Lifetime")]
        [SerializeField] private float lifetimeMultiplierMin = 0.8f;
        [SerializeField] private float lifetimeMultiplierMax = 3f;
        [Range(0f, 1f)]
        [SerializeField] private float immortalChance = 0.1f; // 10% chance to live forever
        
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
        private float _actualLifetime = -1f; // -1 means immortal
        private float _normalizedAge => _actualLifetime > 0 ? (_age / _actualLifetime) : 0f;
        
        // Flight parameters
        private Vector3 _noiseOffset;
        private Vector3 _focalPoint;
        
        // Flocking parameters
        private bool _isInFlock = false;
        private float _flockBlendFactor = 0f;
        private float _flockStartTime = 0f;
        private float _breakOutCheckTimer = 0f;
        private Vector3 _flockVelocity = Vector3.zero;
        private readonly List<Butterfly> _nearbyButterflies = new List<Butterfly>();
        
        // Debug tracking
        private float _debugLogTimer = 0f;
        private Vector3 _lastPosition;
        private float _stuckTimer = 0f;
        private const float STUCK_THRESHOLD = 0.01f; // Movement less than this in 1 second = stuck
        
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
            
            if (energySystem == null)
                energySystem = GetComponent<ButterflyEnergy>();
            
            if (pollinationSystem == null)
                pollinationSystem = GetComponent<ButterflyPollination>();
            
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
            // Initialize focal point - use spawn position (will be updated during Initialize if needed)
            _focalPoint = transform.position;
            
            // Ensure focal point is not zero (could cause issues)
            if (_focalPoint.sqrMagnitude < 0.01f)
            {
                _focalPoint = Vector3.zero;
            }
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
            
            // Calculate random lifetime (some may be immortal)
            if (Random.value < immortalChance)
            {
                _actualLifetime = -1f; // -1 means immortal (never dies)
                if (enableDebugLogs)
                    Debug.Log($"[Butterfly] {gameObject.name}: IMMORTAL butterfly spawned!");
            }
            else
            {
                float lifetimeMultiplier = Random.Range(lifetimeMultiplierMin, lifetimeMultiplierMax);
                _actualLifetime = archetype.lifetime * lifetimeMultiplier;
                if (enableDebugLogs)
                    Debug.Log($"[Butterfly] {gameObject.name}: Spawned with lifetime {_actualLifetime:F1}s (base={archetype.lifetime:F1}s, multiplier={lifetimeMultiplier:F2}x)");
            }
            
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
            // Ensure velocity is not zero
            if (_velocity.sqrMagnitude < 0.01f)
            {
                _velocity = Vector3.forward * archetype.flightSpeedCurve.Evaluate(0f);
            }
            
            // Set focal point to spawn position (butterflies orbit around where they spawn)
            _focalPoint = transform.position;
            if (_focalPoint.sqrMagnitude < 0.01f)
            {
                _focalPoint = Vector3.zero;
            }
            
            _lastPosition = transform.position; // Initialize for debug tracking
            
            if (enableDebugLogs)
                Debug.Log($"[Butterfly] {gameObject.name}: Initialized at {transform.position:F2}, velocity={_velocity:F3}, speed={_velocity.magnitude:F3}, " +
                         $"focalPoint={_focalPoint:F2}, lifetime={(_actualLifetime < 0 ? "IMMORTAL" : _actualLifetime.ToString("F1") + "s")}");
            
            StartCoroutine(LifecycleCoroutine());
        }
        
        private IEnumerator LifecycleCoroutine()
        {
            // Emerging phase
            yield return StartCoroutine(EmergingCoroutine());
            
            // Main flying loop
            _currentState = State.Flying;
            
            // Loop while flying and not expired (immortal butterflies never expire)
            while (_currentState == State.Flying && (_actualLifetime < 0 || _age < _actualLifetime))
            {
                yield return null;
            }
            
            // Check if we should dissipate or land
            if (_currentState == State.Flying)
            {
                if (enableDebugLogs)
                    Debug.Log($"[Butterfly] {gameObject.name}: Lifetime expired in coroutine (age={_age:F1}s, lifetime={_actualLifetime:F1}s)");
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
            if (enableDebugLogs)
                Debug.Log($"[Butterfly] {gameObject.name}: Starting dissipation (age={_age:F1}s, lifetime={_archetype.lifetime:F1}s)");
            
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
            
            if (enableDebugLogs)
                Debug.Log($"[Butterfly] {gameObject.name}: Destroying butterfly");
            
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
            // Check if lifetime expired (immortal butterflies have _actualLifetime = -1)
            if (_actualLifetime > 0 && _age >= _actualLifetime && _currentState == State.Flying)
            {
                if (enableDebugLogs)
                    Debug.Log($"[Butterfly] {gameObject.name}: Lifetime expired in UpdateState (age={_age:F1}s, lifetime={_actualLifetime:F1}s)");
                _currentState = State.Dissipating;
                StartCoroutine(DissipatingCoroutine());
            }
        }
        
        private void UpdateFlying()
        {
            if (_archetype == null)
            {
                if (enableDebugLogs)
                    Debug.LogError($"[Butterfly] {gameObject.name}: Cannot update flying - archetype is null!");
                return;
            }
            
            float t = Time.time;
            float speed = _archetype.flightSpeedCurve.Evaluate(_normalizedAge);
            
            // Always calculate individual flight path (Perlin noise-based wandering)
            Vector3 individualDir = CalculateIndividualFlightPath(t, speed);
            
            if (enableDebugLogs && _debugLogTimer < 0.1f) // Log only occasionally
            {
                if (individualDir.sqrMagnitude < 0.01f)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: Individual direction is zero!");
            }
            
            // Check for flocking behavior
            Vector3 flockDir = Vector3.zero;
            bool shouldFlock = false;
            bool hasValidFlockDir = false;
            
            if (enableFlocking && _currentState == State.Flying)
            {
                shouldFlock = CheckForFlocking();
                
                // Check for breaking out of flock first
                if (_isInFlock)
                {
                    CheckForBreakOut();
                    
                    // Re-check if we should still flock after breakout check
                    shouldFlock = CheckForFlocking() && _isInFlock;
                }
                
                if (shouldFlock || _isInFlock)
                {
                    flockDir = CalculateFlockingDirection();
                    
                    // Only use flock direction if it's valid (not zero)
                    if (flockDir.sqrMagnitude > 0.01f)
                    {
                        hasValidFlockDir = true;
                        
                        // Blend into flock
                        if (!_isInFlock)
                        {
                            _isInFlock = true;
                            _flockStartTime = Time.time;
                            if (enableDebugLogs)
                                Debug.Log($"[Butterfly] {gameObject.name}: JOINED FLOCK (nearby: {_nearbyButterflies.Count})");
                        }
                        
                        _flockBlendFactor = Mathf.Lerp(_flockBlendFactor, 1f, Time.deltaTime * flockBlendSpeed);
                    }
                    else
                    {
                        // Invalid flock direction, break out
                        hasValidFlockDir = false;
                        if (_isInFlock)
                        {
                            if (enableDebugLogs)
                                Debug.Log($"[Butterfly] {gameObject.name}: LEFT FLOCK (invalid direction, dir={flockDir:F3})");
                        }
                        _isInFlock = false;
                        _flockBlendFactor = Mathf.Lerp(_flockBlendFactor, 0f, Time.deltaTime * flockBlendSpeed * 2f);
                    }
                }
                else
                {
                    // Blend out of flock
                    hasValidFlockDir = false;
                    _flockBlendFactor = Mathf.Lerp(_flockBlendFactor, 0f, Time.deltaTime * flockBlendSpeed * 2f);
                    
                    if (_flockBlendFactor < 0.1f && _isInFlock)
                    {
                        _isInFlock = false;
                        if (enableDebugLogs)
                            Debug.Log($"[Butterfly] {gameObject.name}: LEFT FLOCK (no nearby butterflies)");
                    }
                }
            }
            else
            {
                // Flocking disabled or not flying, ensure we're not in flock
                if (_isInFlock)
                {
                    _isInFlock = false;
                    if (enableDebugLogs)
                        Debug.Log($"[Butterfly] {gameObject.name}: LEFT FLOCK (flocking disabled or not flying)");
                }
                _flockBlendFactor = 0f;
            }
            
            // Blend individual and flock directions
            // If no valid flock direction, always use individual
            Vector3 finalDir;
            if (hasValidFlockDir && _flockBlendFactor > 0.01f)
            {
                finalDir = Vector3.Lerp(individualDir, flockDir, _flockBlendFactor);
            }
            else
            {
                // Use individual path if no valid flock or blend factor is low
                finalDir = individualDir;
                _flockBlendFactor = 0f;
            }
            
            // Constrain to flight radius
            Vector3 toFocal = transform.position - _focalPoint;
            float distance = toFocal.magnitude;
            if (distance > _archetype.maxFlightRadius)
            {
                Vector3 pullBack = -toFocal.normalized * (distance - _archetype.maxFlightRadius) * 0.5f;
                finalDir += pullBack;
                if (enableDebugLogs && _debugLogTimer < 0.1f)
                    Debug.Log($"[Butterfly] {gameObject.name}: Pulled back (distance={distance:F2} > max={_archetype.maxFlightRadius:F2})");
            }
            else if (distance < _archetype.minFlightRadius)
            {
                Vector3 pushOut = toFocal.normalized * (_archetype.minFlightRadius - distance) * 0.5f;
                finalDir += pushOut;
                if (enableDebugLogs && _debugLogTimer < 0.1f)
                    Debug.Log($"[Butterfly] {gameObject.name}: Pushed out (distance={distance:F2} < min={_archetype.minFlightRadius:F2})");
            }
            
            // Constrain to bounding box with surface avoidance
            if (ButterflyManager.Instance != null && ButterflyManager.Instance.UseBoundingBox)
            {
                Vector3 position = transform.position;
                float steerStrength;
                Vector3 boundarySteer = ButterflyManager.Instance.GetBoundarySteerDirection(position, out steerStrength);
                
                if (boundarySteer.sqrMagnitude > 0.01f)
                {
                    // Butterfly is outside or near boundary - steer away from it
                    finalDir += boundarySteer * steerStrength;
                    
                    // Check if velocity is parallel to a wall/floor/ceiling (dragging along surface)
                    if (_velocity.sqrMagnitude > 0.01f)
                    {
                        Vector3 velNormalized = _velocity.normalized;
                        float alignment = Vector3.Dot(velNormalized, boundarySteer);
                        
                        // If velocity is perpendicular to the steering direction (moving along surface),
                        // add extra steering perpendicular to the surface
                        if (Mathf.Abs(alignment) < 0.3f && steerStrength > ButterflyManager.Instance.BoundarySteerStrength * 1.5f)
                        {
                            // Butterfly is dragging along a surface - add perpendicular push
                            Vector3 perpendicularPush = boundarySteer * 2f;
                            
                            // If near ground, ensure upward component
                            float distToGround = position.y - ButterflyManager.Instance.BoundingBoxMin.y;
                            if (distToGround < ButterflyManager.Instance.BoundaryBufferZone * 1.5f && boundarySteer.y > 0.1f)
                            {
                                perpendicularPush.y += ButterflyManager.Instance.GroundUpwardBias;
                            }
                            
                            finalDir += perpendicularPush;
                            
                            if (enableDebugLogs && _debugLogTimer < 0.1f)
                            {
                                Debug.Log($"[Butterfly] {gameObject.name}: Dragging along surface - applying perpendicular push (pos={position:F2}, vel={velNormalized:F2}, boundary={boundarySteer:F2})");
                            }
                        }
                    }
                    
                    if (enableDebugLogs && _debugLogTimer < 0.1f)
                    {
                        Debug.Log($"[Butterfly] {gameObject.name}: Steering from boundary (pos={position:F2}, steer={boundarySteer:F2}, strength={steerStrength:F2})");
                    }
                }
            }
            
            // Ensure we always have a valid direction (fallback to current velocity if somehow zero)
            if (finalDir.sqrMagnitude < 0.01f)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: Final direction is zero! Using fallback. " +
                                   $"Individual={individualDir:F3}, Flock={flockDir:F3}, Blend={_flockBlendFactor:F2}");
                finalDir = _velocity.normalized;
                if (finalDir.sqrMagnitude < 0.01f)
                {
                    finalDir = transform.forward;
                    if (finalDir.sqrMagnitude < 0.01f)
                        finalDir = Vector3.forward; // Last resort
                }
            }
            
            // Update velocity
            Vector3 targetDir = finalDir.normalized;
            _velocity = Vector3.Lerp(_velocity, targetDir * speed, Time.deltaTime * _archetype.turnSpeed);
            
            // Check for zero velocity after update
            if (_velocity.sqrMagnitude < 0.001f)
            {
                if (enableDebugLogs && _stuckTimer > 0.5f)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: Velocity is zero! Speed={speed:F3}, TargetDir={targetDir:F3}, TurnSpeed={_archetype.turnSpeed:F2}");
                _velocity = targetDir * speed * 0.1f; // Force some movement
            }
            
            // Apply movement
            Vector3 newPosition = transform.position + _velocity * Time.deltaTime;
            
            // Clamp to bounding box if enabled
            if (ButterflyManager.Instance != null && ButterflyManager.Instance.UseBoundingBox)
            {
                newPosition = ButterflyManager.Instance.ClampToBounds(newPosition);
            }
            
            transform.position = newPosition;
            
            // Update rotation to face movement direction
            if (_velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _archetype.turnSpeed);
            }
        }
        
        /// <summary>
        /// Calculate individual flight path using Perlin noise.
        /// </summary>
        private Vector3 CalculateIndividualFlightPath(float t, float speed)
        {
            if (_archetype == null)
            {
                if (enableDebugLogs && _debugLogTimer < 0.1f)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: Cannot calculate flight path - archetype is null!");
                return Vector3.forward; // Fallback
            }
            
            Vector3 noiseDir = new Vector3(
                Mathf.PerlinNoise(t * _archetype.noiseScale + _noiseOffset.x, _noiseOffset.y) - 0.5f,
                Mathf.PerlinNoise(_noiseOffset.z, t * _archetype.noiseScale + _noiseOffset.x) - 0.5f,
                Mathf.PerlinNoise(t * _archetype.noiseScale + _noiseOffset.y, _noiseOffset.z) - 0.5f
            );
            
            // Audio-linked vertical oscillation
            float audioFrequency = ButterflyAudio.GetCurrentFrequency(this);
            float bob = Mathf.Sin(t * audioFrequency * 0.01f) * 0.01f;
            noiseDir.y += bob;
            
            if (noiseDir.sqrMagnitude < 0.01f)
            {
                if (enableDebugLogs && _debugLogTimer < 0.1f)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: Calculated noiseDir is zero! Returning forward.");
                return Vector3.forward; // Fallback
            }
            
            return noiseDir.normalized;
        }
        
        /// <summary>
        /// Check if there are nearby butterflies to form a flock with.
        /// </summary>
        private bool CheckForFlocking()
        {
            if (ButterflyManager.Instance == null)
            {
                if (enableDebugLogs && _debugLogTimer < 0.1f)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: Cannot check for flocking - ButterflyManager.Instance is null!");
                return false;
            }
            
            _nearbyButterflies.Clear();
            
            // Find nearby butterflies
            var allButterflies = ButterflyManager.Instance.GetActiveButterflies();
            if (allButterflies == null)
            {
                if (enableDebugLogs && _debugLogTimer < 0.1f)
                    Debug.LogWarning($"[Butterfly] {gameObject.name}: GetActiveButterflies() returned null!");
                return false;
            }
            
            foreach (var other in allButterflies)
            {
                if (other == this || other == null)
                    continue;
                
                // Only consider butterflies that are flying (not emerging, landing, or dissipating)
                if (other._currentState != State.Flying)
                    continue;
                
                float distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance <= flockDetectionRadius)
                {
                    _nearbyButterflies.Add(other);
                }
            }
            
            if (enableDebugLogs && _nearbyButterflies.Count > 0 && _debugLogTimer < 0.1f)
            {
                Debug.Log($"[Butterfly] {gameObject.name}: Found {_nearbyButterflies.Count} nearby butterflies within {flockDetectionRadius}m");
            }
            
            // Need at least one other butterfly to form a flock
            return _nearbyButterflies.Count >= 1;
        }
        
        /// <summary>
        /// Calculate flocking direction using cohesion, alignment, and separation.
        /// </summary>
        private Vector3 CalculateFlockingDirection()
        {
            if (_nearbyButterflies.Count == 0)
                return Vector3.zero;
            
            Vector3 cohesion = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 separation = Vector3.zero;
            int separationCount = 0;
            
            foreach (var neighbor in _nearbyButterflies)
            {
                if (neighbor == null || neighbor == this) continue;
                
                Vector3 toNeighbor = neighbor.transform.position - transform.position;
                float distance = toNeighbor.magnitude;
                
                // Cohesion: Move towards center of nearby butterflies
                cohesion += neighbor.transform.position;
                
                // Alignment: Align with velocity of nearby butterflies
                alignment += neighbor._velocity;
                
                // Separation: Avoid getting too close to neighbors
                if (distance < flockSeparationDistance && distance > 0.01f)
                {
                    separation -= toNeighbor.normalized / distance;
                    separationCount++;
                }
            }
            
            // Average cohesion
            if (_nearbyButterflies.Count > 0)
            {
                cohesion = (cohesion / _nearbyButterflies.Count) - transform.position;
                cohesion = cohesion.normalized;
            }
            
            // Average alignment
            if (_nearbyButterflies.Count > 0)
            {
                alignment = alignment.normalized;
            }
            
            // Average separation
            if (separationCount > 0)
            {
                separation = separation.normalized;
            }
            
            // Combine flocking forces
            Vector3 flockDirection = (cohesion * flockCohesionWeight + 
                                     alignment * flockAlignmentWeight + 
                                     separation * flockSeparationWeight).normalized;
            
            return flockDirection;
        }
        
        /// <summary>
        /// Check if butterfly should break out of the flock.
        /// </summary>
        private void CheckForBreakOut()
        {
            if (!_isInFlock) return;
            
            _breakOutCheckTimer += Time.deltaTime;
            
            // Check distance immediately (every frame)
            bool distanceBreakOut = _nearbyButterflies.Count == 0;
            
            if (distanceBreakOut)
            {
                // Immediately break out if no nearby butterflies
                _isInFlock = false;
                _flockStartTime = 0f;
                _flockBlendFactor = Mathf.Max(0f, _flockBlendFactor * 0.5f); // Quick drop
                _breakOutCheckTimer = 0f;
                if (enableDebugLogs)
                    Debug.Log($"[Butterfly] {gameObject.name}: BROKE OUT of flock (distance - no nearby)");
                return;
            }
            
            // Periodic checks for other breakout conditions
            if (_breakOutCheckTimer >= breakOutCheckInterval)
            {
                _breakOutCheckTimer = 0f;
                
                // Break out conditions:
                // 1. Random chance
                bool randomBreakOut = Random.value < breakOutChance;
                
                // 2. Too long in flock
                float timeInFlock = Time.time - _flockStartTime;
                bool timeBreakOut = timeInFlock > maxFlockTime;
                
                if (randomBreakOut || timeBreakOut)
                {
                    // Break out of flock
                    _isInFlock = false;
                    _flockStartTime = 0f;
                    _flockBlendFactor = Mathf.Max(0f, _flockBlendFactor * 0.5f); // Quick drop
                    if (enableDebugLogs)
                        Debug.Log($"[Butterfly] {gameObject.name}: BROKE OUT of flock (random={randomBreakOut}, time={timeBreakOut}, timeInFlock={timeInFlock:F1}s)");
                }
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
            
            // Check if butterfly needs energy (prioritize fruits when low energy)
            bool needsEnergy = energySystem != null && energySystem.NeedsEnergy;
            bool isCarryingPollen = pollinationSystem != null && pollinationSystem.IsCarryingPollen;
            
            // Determine seek chance based on state
            float seekChance = 0.3f;
            if (needsEnergy) seekChance = 0.7f; // Higher chance if low energy
            if (isCarryingPollen) seekChance = 0.6f; // Higher chance if carrying pollen
            
            // Random chance to seek landing target
            if (Random.value > seekChance) return;
            
            Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, landingRadius, landingTargetLayer);
            
            // Prioritize targets based on state
            // - Low energy: prefer flowers or fruits
            // - Carrying pollen: prefer fruit (to deposit) or flowers (to pollinate)
            // - Otherwise: prefer fruits, then flowers, then other targets
            Interaction.LandingTarget fruitTarget = null;
            Interaction.LandingTarget flowerTarget = null;
            Interaction.LandingTarget otherTarget = null;
            
            foreach (var target in nearbyTargets)
            {
                var landingTarget = target.GetComponent<Interaction.LandingTarget>();
                if (landingTarget == null || !landingTarget.IsAvailable) continue;
                
                // Skip if this is the last place we landed (unless we've landed somewhere else since)
                if (landingTarget == _lastLandingTarget) continue;
                
                // Check target type
                if (landingTarget.Type == Interaction.LandingTarget.TargetType.Fruit)
                {
                    fruitTarget = landingTarget;
                    // Don't break - continue to check for flowers if we're carrying pollen
                    if (!isCarryingPollen) break; // Otherwise prioritize fruits
                }
                else if (landingTarget.Type == Interaction.LandingTarget.TargetType.Plant)
                {
                    // Check if it's actually a flower
                    Flower flower = target.GetComponent<Flower>();
                    if (flower == null && target.transform.parent != null)
                        flower = target.transform.parent.GetComponent<Flower>();
                    
                    if (flower != null)
                    {
                        flowerTarget = landingTarget;
                        // Prefer flowers when low energy or not carrying pollen
                        if (needsEnergy || !isCarryingPollen) break;
                    }
                    else if (otherTarget == null)
                    {
                        otherTarget = landingTarget;
                    }
                }
                else if (otherTarget == null)
                {
                    otherTarget = landingTarget;
                }
            }
            
            // Choose target based on state
            Interaction.LandingTarget chosenTarget = null;
            
            if (needsEnergy)
            {
                // Low energy: prefer flowers (collect pollen + energy) or fruits
                chosenTarget = flowerTarget ?? fruitTarget;
            }
            else if (isCarryingPollen)
            {
                // Carrying pollen: prefer fruit (deposit pollen) or flowers (pollinate)
                chosenTarget = fruitTarget ?? flowerTarget;
            }
            else
            {
                // Normal state: prefer fruit, then flower, then other
                chosenTarget = fruitTarget ?? flowerTarget ?? otherTarget;
            }
            
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
            
            // Note: Fruit and flower notifications happen in UpdateLanding() after landing completes
            // to handle energy feeding, pollen collection/deposition correctly
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
                
                // Check if landing on fruit and feed energy / deposit pollen
                Plants.GenerativeFruit fruit = null;
                if (_currentLandingTarget.Type == Interaction.LandingTarget.TargetType.Fruit)
                {
                    fruit = _currentLandingTarget.GetComponent<Plants.GenerativeFruit>();
                    if (fruit == null && _currentLandingTarget.transform.parent != null)
                    {
                        fruit = _currentLandingTarget.transform.parent.GetComponent<Plants.GenerativeFruit>();
                    }
                    
                    if (fruit != null)
                    {
                        // Feed energy from fruit
                        if (energySystem != null)
                        {
                            energySystem.FeedFromFruit(fruit);
                            fruit.OnButterflyFeeding(this);
                        }
                        
                        // Deposit pollen to fruit (accelerates fruit evolution)
                        if (pollinationSystem != null && pollinationSystem.IsCarryingPollen)
                        {
                            pollinationSystem.DepositPollenToFruit(fruit);
                        }
                    }
                }
                
                // Check if landing on flower and feed energy / collect pollen / deposit pollen
                Flower flower = null;
                if (_currentLandingTarget.Type == Interaction.LandingTarget.TargetType.Plant)
                {
                    flower = _currentLandingTarget.GetComponent<Flower>();
                    if (flower == null && _currentLandingTarget.transform.parent != null)
                    {
                        flower = _currentLandingTarget.transform.parent.GetComponent<Flower>();
                    }
                    
                    if (flower != null)
                    {
                        // On landing, butterfly feeds and collects pollen OR deposits pollen
                        if (pollinationSystem != null && pollinationSystem.IsCarryingPollen)
                        {
                            // Deposit pollen (cross-pollination)
                            pollinationSystem.DepositPollen(flower);
                        }
                        else
                        {
                            // Feed and collect pollen
                            flower.OnButterflyLanded(this);
                        }
                    }
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
                    // Notify ecosystem orchestrator
                    if (Core.EcosystemOrchestrator.Instance != null)
                    {
                        Core.EcosystemOrchestrator.Instance.RegisterButterflyLandingOnHand(this);
                    }
                    
                    // Also notify ecosystem state controller for compatibility
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

