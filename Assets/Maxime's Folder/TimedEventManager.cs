using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TimedEventManager : Singleton<TimedEventManager>
{

    [System.Serializable]
    public struct TimedEventEntry
    {
        public TimedEvent timedEvent;
        public bool hasPlayed;
    }
    [SerializeField] private TimedEvent[] TimedEventList;

    private AudioSource audioSource;

    private TimedEventEntry[] timedEventEntries;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Initialiser l'array de paires
        timedEventEntries = new TimedEventEntry[TimedEventList.Length];

        for (int i = 0; i < TimedEventList.Length; i++)
        {
            timedEventEntries[i] = new TimedEventEntry
            {
                timedEvent = TimedEventList[i],
                hasPlayed = false
            };

            Debug.Log("Added : " + TimedEventList[i]);
        }
    }

    void Update()
    {
        CheckTimedEvents();
    }

    void CheckTimedEvents()
    {
        float timer = GameOutcomeManager.Instance.GetTimerProgression();
        //Debug.Log("Timer: " + timer);

        for (int i = 0; i < timedEventEntries.Length; i++)
        {
            //Debug.Log($"Checking on {timedEventEntries[i].timedEvent.name} if {!timedEventEntries[i].hasPlayed} && {timer} >= {timedEventEntries[i].timedEvent.RunDurationPercentageStart}");
            if (!timedEventEntries[i].hasPlayed &&
                timer >= timedEventEntries[i].timedEvent.RunDurationPercentageStart)
            {
                Debug.Log("Play : " + timedEventEntries[i].timedEvent.audioClip);

                audioSource.PlayOneShot(
                    timedEventEntries[i].timedEvent.audioClip, 0.3f
                );

                // réassigner la struct modifiée
                var entry = timedEventEntries[i];
                entry.hasPlayed = true;
                timedEventEntries[i] = entry;
            }
        }
    }
}