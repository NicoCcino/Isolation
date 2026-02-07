using UnityEngine;
[System.Serializable]
public class HuntingState : AEnemyState
{
    public HuntingState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        Debug.Log("Agent is in hunting state...");
    }
}
