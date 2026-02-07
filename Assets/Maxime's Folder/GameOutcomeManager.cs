using System;
using UnityEngine;


public class GameOutcomeManager : Singleton<GameOutcomeManager>
{
    public bool m_ShowDebugLog = false;
    public bool m_ShowDebugTimer = false;

    public enum E_GameOutcome
    {
        None,
        Success,
        Defeat
    }

    private E_GameOutcome GameOutcome = E_GameOutcome.None;

    private float Timer = 0;
    public float GameDuration = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(m_ShowDebugLog) Debug.Log("Init GameOutcomeManager");
        //Reset state variable
        Timer = 0;
        GameOutcome = E_GameOutcome.None;
    }

    public void Victory(string SuccessReason)
    {
        GameOutcome = E_GameOutcome.Success;
        Debug.Log("Succes");
        if(m_ShowDebugLog) Debug.Log("VICTORY : " + SuccessReason + " / " + GameOutcome);
    }

    public void Defeat(string DefeatReason) 
    {
        GameOutcome = E_GameOutcome.Defeat;
        if(m_ShowDebugLog) Debug.Log("DEFEAT : " + DefeatReason + " / " + GameOutcome);
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();
        if (Timer >= GameDuration)
        {
            Defeat("Timer Elasped");
        }
    }

    void TimerUpdate()
    {
        Timer+= Time.deltaTime;
            if(m_ShowDebugTimer) Debug.Log("Timer : "+ Timer + "s" );       
    }

    public float GetTimer()
    {
        return Timer;
    }

    public int GetTimerProgression()
    {
        int TimerProgression = 0;
        TimerProgression = (int) Math.Round(Timer/GameDuration);
        return TimerProgression;
    }
}

