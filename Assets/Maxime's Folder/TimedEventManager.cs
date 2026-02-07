using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : Singleton<TimedEventManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TimedEvent[] TimedEventList;
    [SerializeField] private AudioLoopBlender audioLoopBlender;
    private Dictionary<TimedEvent,Boolean> TimedEventPlayMap; 
    void Start()
    {
        foreach (TimedEvent timedEvent in TimedEventList)
        {
            TimedEventPlayMap.Add(timedEvent,false);
        }
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
                
                    audioLoopBlender.PlayOverrideEvent(timedEvent.audioClip);
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
