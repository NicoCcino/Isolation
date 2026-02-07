using UnityEngine;
public abstract class AEnemyState : BaseState
{

    protected EnemyController enemyController;
    protected EnemyVision enemyVision;
    protected EnemyData enemyData;
    protected EnemyStateManager enemyStateManager;

    protected AEnemyState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager)
    {
        this.enemyController = enemyController;
        this.enemyVision = enemyVision;
        this.enemyData = enemyData;
        this.enemyStateManager = enemyStateManager;
    }

    public override abstract void Enter();
    public override abstract void Exit();
    public override abstract void Update();

}
