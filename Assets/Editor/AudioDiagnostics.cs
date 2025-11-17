using UnityEngine;
using UnityEditor;
using ButterflyHouse.Butterflies;
using ButterflyHouse.Audio;
using ButterflyHouse.Core;

namespace ButterflyHouse.Editor
{
    /// <summary>
    /// Diagnostic tool to check audio setup and identify why audio might not be playing.
    /// </summary>
    public class AudioDiagnostics : EditorWindow
    {
        [MenuItem("Butterfly House/Diagnose Audio Issues", false, 20)]
        public static void ShowWindow()
        {
            Debug.Log("=== Audio Diagnostics ===");
            
            // Check AudioListener
            AudioListener listener = Object.FindObjectOfType<AudioListener>();
            if (listener == null)
            {
                Debug.LogError("❌ NO AUDIO LISTENER FOUND! This is likely why you can't hear audio.");
                Debug.LogError("   Solution: Add AudioListener component to your Main Camera.");
            }
            else
            {
                Debug.Log($"✓ AudioListener found on: {listener.gameObject.name}");
            }
            
            // Check AudioManager
            Audio.AudioManager audioManager = Object.FindObjectOfType<Audio.AudioManager>();
            if (audioManager == null)
            {
                Debug.LogWarning("⚠ AudioManager not found in scene");
            }
            else
            {
                Debug.Log($"✓ AudioManager found");
                SerializedObject so = new SerializedObject(audioManager);
                SerializedProperty masterVol = so.FindProperty("masterVolume");
                if (masterVol != null)
                {
                    Debug.Log($"  Master Volume: {masterVol.floatValue:F2}");
                }
            }
            
            // Check Settings
            if (Settings.Instance == null)
            {
                Debug.LogWarning("⚠ Settings.Instance is null (this is OK if not using singleton)");
            }
            else
            {
                Debug.Log($"✓ Settings found");
                Debug.Log($"  Butterfly Volume: {Settings.Instance.butterflyVolume:F2}");
                Debug.Log($"  Master Volume: {Settings.Instance.masterVolume:F2}");
            }
            
            // Check all butterflies in scene
            Butterfly[] butterflies = Object.FindObjectsOfType<Butterfly>();
            Debug.Log($"\nFound {butterflies.Length} butterfly(ies) in scene:");
            
            int butterfliesWithAudio = 0;
            int butterfliesWithoutAudio = 0;
            int butterfliesPlaying = 0;
            int butterfliesNotPlaying = 0;
            
            foreach (Butterfly butterfly in butterflies)
            {
                ButterflyAudio butterflyAudio = butterfly.GetComponent<ButterflyAudio>();
                if (butterflyAudio == null)
                {
                    butterfliesWithoutAudio++;
                    Debug.LogWarning($"  ❌ {butterfly.name}: No ButterflyAudio component!");
                    continue;
                }
                
                butterfliesWithAudio++;
                
                SerializedObject audioSO = new SerializedObject(butterflyAudio);
                SerializedProperty audioSourceProp = audioSO.FindProperty("audioSource");
                
                if (audioSourceProp != null && audioSourceProp.objectReferenceValue != null)
                {
                    AudioSource audioSource = audioSourceProp.objectReferenceValue as AudioSource;
                    if (audioSource != null)
                    {
                        string status = "";
                        
                        if (audioSource.clip == null)
                        {
                            status += "❌ No clip assigned | ";
                        }
                        else
                        {
                            status += $"✓ Clip: {audioSource.clip.name} | ";
                        }
                        
                        if (audioSource.isPlaying)
                        {
                            butterfliesPlaying++;
                            status += "✓ Playing | ";
                        }
                        else
                        {
                            butterfliesNotPlaying++;
                            status += "❌ NOT PLAYING | ";
                        }
                        
                        status += $"Volume: {audioSource.volume:F3} | ";
                        status += $"Mute: {audioSource.mute} | ";
                        status += $"Enabled: {audioSource.enabled} | ";
                        status += $"Spatial: {audioSource.spatialBlend:F1}";
                        
                        Debug.Log($"  {butterfly.name}: {status}");
                        
                        // Check archetype
                        SerializedProperty archProp = audioSO.FindProperty("_archetype");
                        if (archProp != null && archProp.objectReferenceValue != null)
                        {
                            ButterflyArchetype arch = archProp.objectReferenceValue as ButterflyArchetype;
                            if (arch != null)
                            {
                                SerializedObject archSO = new SerializedObject(arch);
                                SerializedProperty baseToneProp = archSO.FindProperty("baseTone");
                                if (baseToneProp != null && baseToneProp.objectReferenceValue == null)
                                {
                                    Debug.LogWarning($"    ⚠ ButterflyArchetype '{arch.name}' has no Base Tone assigned!");
                                }
                            }
                        }
                    }
                }
            }
            
            Debug.Log($"\nSummary:");
            Debug.Log($"  Butterflies with audio: {butterfliesWithAudio}");
            Debug.Log($"  Butterflies without audio: {butterfliesWithoutAudio}");
            Debug.Log($"  Butterflies playing: {butterfliesPlaying}");
            Debug.Log($"  Butterflies not playing: {butterfliesNotPlaying}");
            
            // Check ButterflyArchetypes
            string[] archetypeGuids = AssetDatabase.FindAssets("t:ButterflyArchetype");
            int archetypesWithAudio = 0;
            int archetypesWithoutAudio = 0;
            
            foreach (string guid in archetypeGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ButterflyArchetype arch = AssetDatabase.LoadAssetAtPath<ButterflyArchetype>(path);
                
                SerializedObject archSO = new SerializedObject(arch);
                SerializedProperty baseToneProp = archSO.FindProperty("baseTone");
                
                if (baseToneProp != null && baseToneProp.objectReferenceValue != null)
                {
                    archetypesWithAudio++;
                }
                else
                {
                    archetypesWithoutAudio++;
                    Debug.LogWarning($"⚠ ButterflyArchetype '{arch.name}' has no Base Tone assigned");
                }
            }
            
            Debug.Log($"\nButterflyArchetypes:");
            Debug.Log($"  With audio: {archetypesWithAudio}");
            Debug.Log($"  Without audio: {archetypesWithoutAudio}");
            
            // Recommendations
            Debug.Log("\n=== Recommendations ===");
            
            if (listener == null)
            {
                Debug.LogError("1. CRITICAL: Add AudioListener to Main Camera!");
            }
            
            if (archetypesWithoutAudio > 0)
            {
                Debug.LogWarning("2. Assign AudioClips to ButterflyArchetype assets (Base Tone field)");
            }
            
            if (butterfliesNotPlaying > 0 && butterfliesWithAudio > 0)
            {
                Debug.LogWarning("3. Some butterflies have audio but aren't playing. Check:");
                Debug.LogWarning("   - Are AudioClips assigned to ButterflyArchetypes?");
                Debug.LogWarning("   - Is AudioSource enabled and not muted?");
                Debug.LogWarning("   - Is volume > 0?");
            }
            
            if (Settings.Instance != null && Settings.Instance.butterflyVolume < 0.1f)
            {
                Debug.LogWarning("4. Butterfly volume in Settings is very low. Consider increasing it.");
            }
            
            Debug.Log("=== End Diagnostics ===");
        }
    }
}

