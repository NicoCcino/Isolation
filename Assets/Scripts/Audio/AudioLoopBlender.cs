using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AudioLayer
{
    public string name = "Layer";
    public AudioClip clip;
    [Range(0f, 1f)] public float individualVolume = 1f;
    [HideInInspector] public AudioSource source;
}

public class AudioLoopBlender : MonoBehaviour
{
    [Header("Engine Blend")]
    [Tooltip("0 = First Clip, 1 = Last Clip")]
    [Range(0f, 1f)] public float blendValue = 0f;
    [Range(0f, 1f)] public float masterVolume = 1f;

    [Header("Pitch Settings")]
    public bool usePitchModulation = false;
    [Range(0.8f, 3f)] public float maxPitch = 1.2f;

    [Header("Event Settings")]
    [Tooltip("Number of simultaneous events allowed")]
    public int eventPoolSize = 4;
    public float eventFadeSpeed = 3.0f;

    [Header("Audio Clips")]
    public List<AudioLayer> audioLayers = new List<AudioLayer>();

    // Internal Systems
    private List<AudioSource> eventPool = new List<AudioSource>();
    private List<EventTracker> activeEvents = new List<EventTracker>();

    // Helper class to track the fade state of each playing sound
    private class EventTracker
    {
        public float weight; // 0.0 to 1.0 (How much this event is suppressing the engine)
    }

    private void Start()
    {
        InitializeLoopSources();
        InitializeEventPool();
    }

    private void InitializeLoopSources()
    {
        foreach (var layer in audioLayers)
        {
            if (layer.clip == null) continue;
            GameObject child = new GameObject("EngineLoop_" + layer.name);
            child.transform.SetParent(transform);
            child.transform.localPosition = Vector3.zero;

            layer.source = child.AddComponent<AudioSource>();
            layer.source.clip = layer.clip;
            layer.source.loop = true;
            layer.source.playOnAwake = false;
            layer.source.spatialBlend = 0f;
            layer.source.dopplerLevel = 0f;
            layer.source.volume = 0f;
            layer.source.Play();
        }
    }

    private void InitializeEventPool()
    {
        for (int i = 0; i < eventPoolSize; i++)
        {
            GameObject child = new GameObject($"EventSource_{i}");
            child.transform.SetParent(transform);
            child.transform.localPosition = Vector3.zero;

            AudioSource src = child.AddComponent<AudioSource>();
            src.loop = false;
            src.spatialBlend = 0f;
            src.playOnAwake = false;
            eventPool.Add(src);
        }
    }

    private void Update()
    {
        UpdateVolumes();
        if (usePitchModulation) UpdatePitch();
    }

    // ---------------------------------------------------------
    // PUBLIC API
    // ---------------------------------------------------------

    public void PlayOverrideEvent(AudioClip clip, float volume = 1f)
    {
        AudioSource source = GetAvailableSource();
        if (source != null && clip != null)
        {
            StartCoroutine(HandleEvent(source, clip, volume));
        }
        else
        {
            Debug.LogWarning("AudioLoopBlender: No event sources available! Increase Pool Size.");
        }
    }

    // ---------------------------------------------------------
    // INTERNAL LOGIC
    // ---------------------------------------------------------

    private AudioSource GetAvailableSource()
    {
        // Find a source that isn't currently playing
        foreach (var src in eventPool)
        {
            if (!src.isPlaying) return src;
        }
        return null; // Pool exhausted
    }

    private IEnumerator HandleEvent(AudioSource source, AudioClip clip, float targetVolume)
    {
        // Create a tracker so the engine knows to duck
        EventTracker tracker = new EventTracker();
        activeEvents.Add(tracker);

        source.clip = clip;
        source.volume = 0f; // Start silent
        source.Play();

        // 1. Fade In
        while (tracker.weight < 1f)
        {
            tracker.weight += Time.deltaTime * eventFadeSpeed;
            // Clamp to ensure we don't go over 1
            if (tracker.weight > 1f) tracker.weight = 1f;

            source.volume = tracker.weight * targetVolume * masterVolume;
            yield return null;
        }

        // 2. Play Duration
        // Calculate remaining time: Clip Length - (FadeInTime + FadeOutTime)
        float fadeDuration = 1f / eventFadeSpeed;
        float waitTime = clip.length - (fadeDuration * 2);

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // 3. Fade Out
        while (tracker.weight > 0f)
        {
            tracker.weight -= Time.deltaTime * eventFadeSpeed;
            if (tracker.weight < 0f) tracker.weight = 0f;

            source.volume = tracker.weight * targetVolume * masterVolume;
            yield return null;
        }

        source.Stop();
        source.clip = null;
        activeEvents.Remove(tracker);
    }

    private void UpdateVolumes()
    {
        // Calculate how much the engine should be suppressed.
        // We take the MAX weight of all active events. 
        // If Event A is fully playing (1.0) and Event B is just starting (0.2), suppression is 1.0.
        float maxSuppression = 0f;

        if (activeEvents.Count > 0)
        {
            // Linq is slightly slower, doing a manual loop for performance safety in Update
            for (int i = 0; i < activeEvents.Count; i++)
            {
                if (activeEvents[i].weight > maxSuppression)
                    maxSuppression = activeEvents[i].weight;
            }
        }

        // The engine volume is the inverse of the suppression
        float engineMasterVolume = (1f - maxSuppression) * masterVolume;

        // --- Standard Loop Blending Logic ---
        int count = audioLayers.Count;
        if (count == 0) return;

        if (count == 1)
        {
            audioLayers[0].source.volume = engineMasterVolume * audioLayers[0].individualVolume;
            return;
        }

        float scaledValue = Mathf.Clamp(blendValue, 0f, 1f) * (count - 1);
        int lowerIndex = Mathf.FloorToInt(scaledValue);
        int upperIndex = lowerIndex + 1;
        float fraction = scaledValue - lowerIndex;

        for (int i = 0; i < count; i++)
        {
            float targetWeight = 0f;
            if (i == lowerIndex) targetWeight = 1f - fraction;
            else if (i == upperIndex) targetWeight = fraction;

            if (audioLayers[i].source != null)
            {
                audioLayers[i].source.volume = targetWeight * engineMasterVolume * audioLayers[i].individualVolume;
            }
        }
    }

    private void UpdatePitch()
    {
        float pitch = Mathf.Lerp(1f, maxPitch, blendValue);
        foreach (var layer in audioLayers)
        {
            if (layer.source != null)
                layer.source.pitch = pitch;
        }
    }
}