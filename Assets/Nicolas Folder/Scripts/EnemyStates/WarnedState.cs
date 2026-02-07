using UnityEngine;
[System.Serializable]
public class WarnedState : AEnemyState
{

    public float warnedTimer = 0f;
    public float warnedTime = 2f;
    public override void Enter()
    {
        // Stop movement
        enemyController.agent.speed = 0f;
        warnedTimer = 0f;
        // TO DO : Add jump animation and exclamation mark
    }

    public override void Exit()
    {
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
