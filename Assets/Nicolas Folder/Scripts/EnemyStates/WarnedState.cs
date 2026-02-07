using UnityEngine;
using NaughtyAttributes;
[System.Serializable]
public class WarnedState : AEnemyState
{
    protected float warnedTimer = 0f;
    public float warnedTime = 2f;
    public GameObject exclamationMarkGO;

    public WarnedState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager, float warnedTime, GameObject exclamationMarkGO) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
        this.warnedTime = warnedTime;
        this.exclamationMarkGO = exclamationMarkGO;
    }

    public override void Enter()
    {
        // Stop movement
        enemyController.agent.speed = 0f;
        warnedTimer = 0f;
        // TO DO : Add jump animation and exclamation mark
        // Display exclamation mark
        exclamationMarkGO.SetActive(true);

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
