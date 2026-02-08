using UnityEngine;
[System.Serializable]
public class DefaultState : AEnemyState
{
    public DefaultState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
    }

    public override void Enter()
    {
        enemyStateManager.ChangeState(EEnemyState.Patrolling);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {

    }
}
