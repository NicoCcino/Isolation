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

    public static int highScore;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_ShowDebugLog) Debug.Log("Init GameOutcomeManager");
        //Reset state variable
        Timer = 0;
        GameOutcome = E_GameOutcome.None;
    }

    public void Victory(string SuccessReason)
    {
        GameOutcome = E_GameOutcome.Success;
        Debug.Log("Succes");
        float timeLeft = GameDuration - Timer;
        if (highScore < timeLeft)
        {
            highScore = Mathf.RoundToInt(timeLeft);
        }
        if (m_ShowDebugLog) Debug.Log("VICTORY : " + SuccessReason + " / " + GameOutcome);
        Debug.Log("Time left in this run: " + timeLeft);
        Debug.Log("High score: " + highScore);
        GameStateManager.Instance.ChangeState(EGameState.Victory);
    }

    public void Defeat(string DefeatReason)
    {
        GameOutcome = E_GameOutcome.Defeat;
        if (m_ShowDebugLog) Debug.Log("DEFEAT : " + DefeatReason + " / " + GameOutcome);
        GameStateManager.Instance.ChangeState(EGameState.GameOver);
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
        Timer += Time.deltaTime;
        if (m_ShowDebugTimer) Debug.Log("Timer : " + Timer + "s");
    }

    public float GetTimer()
    {
        return Timer;
    }

    public int GetTimerProgression()
    {
        int TimerProgression = 0;
        TimerProgression = (int)Math.Round(100 * Timer / GameDuration);
        return TimerProgression;
    }
}

