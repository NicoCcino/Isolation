using UnityEngine;


public class GameOutcomeManager : Singleton<GameOutcomeManager>
{
    public static GameOutcomeManager instance;
    public bool m_ShowDebugLog = false;

    private enum E_GameOutcome
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
        if(m_ShowDebugLog) Debug.Log("VICTORY" + SuccessReason + " / " + GameOutcome);
    }

    public void Defeat(string DefeatReason) 
    {
        GameOutcome = E_GameOutcome.Defeat;
        if(m_ShowDebugLog) Debug.Log("Defeat" + DefeatReason + " / " + GameOutcome);
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();
        if (Timer >= GameDuration)
        {
            Defeat("TimerElasped");
        }
    }

    void TimerUpdate()
    {
        Timer++;
        if(m_ShowDebugLog) Debug.Log("Timer : "+ Timer);
    }
}

