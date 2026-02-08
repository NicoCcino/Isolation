using UnityEngine;
using NaughtyAttributes;
[System.Serializable]
public class WarnedState : AEnemyState
{
    protected float warnedTimer = 0f;
    public float warnedTime = 2f;
    public GameObject exclamationMarkGO;
    protected EnemyAudio enemyAudio;


    public WarnedState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager, EnemyAudio enemyAudio, float warnedTime, GameObject exclamationMarkGO) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
        this.warnedTime = warnedTime;
        this.exclamationMarkGO = exclamationMarkGO;
        this.enemyAudio = enemyAudio;
    }

    public override void Enter()
    {
        // Stop movement
        enemyController.agent.speed = 0f;
        warnedTimer = 0f;
        // Display exclamation mark
        exclamationMarkGO.SetActive(true);
        // TODO: Add jump action on NPC
        // Play sound
        enemyAudio.PlayWarnedSound();
        

    }

    public override void Exit()
    {
        // Hide exclamation mark
        exclamationMarkGO.SetActive(false);
    }

    public override void Update()
    {
        Debug.Log("Agent is in warned state...");
        warnedTimer += Time.deltaTime;
        if (warnedTimer >= warnedTime)
        {
            // Enemy has been detecting player for long enough, switching to Attack State
            enemyStateManager.ChangeState(EEnemyState.Attacking);
        }

    }
}
