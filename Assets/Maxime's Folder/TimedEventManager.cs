using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TimedEventManager : Singleton<TimedEventManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TimedEvent[] TimedEventList;
    [SerializeField] private AudioSource audioSource;
    private Dictionary<TimedEvent,bool> TimedEventPlayMap; 
    void Start()
    {
        foreach (TimedEvent timedEvent in TimedEventList)
        {
            TimedEventPlayMap.Add(timedEvent,false);
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(TimedEvent timedEvent in TimedEventPlayMap.Keys)
        {
            if (TimedEventPlayMap[timedEvent] == false
                && GameOutcomeManager.Instance.GetTimerProgression() >= timedEvent.RunDurationPercentageStart 
                && GameOutcomeManager.Instance.GetTimerProgression()<= timedEvent.RunDurationPercentageStop)
            {
                    audioSource.clip = timedEvent.audioClip;
                    audioSource.Play();
                    TimedEventPlayMap[timedEvent]=true; 
                             
            }
            if(TimedEventPlayMap[timedEvent] == true 
                && GameOutcomeManager.Instance.GetTimerProgression() >= timedEvent.RunDurationPercentageStop)
            {
                //StopSound
                TimedEventPlayMap[timedEvent] = false;
            }
        }
    }
}
