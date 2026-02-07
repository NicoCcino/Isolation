using UnityEngine;
public abstract class AEnemyState : BaseState
{

    [SerializeField] public EnemyController enemyController;
    [SerializeField] public EnemyVision enemyVision;
    [SerializeField] public EnemyData enemyData;
    [SerializeField] public EnemyStateManager enemyStateManager;

    public override abstract void Enter();
    public override abstract void Exit();
    public override abstract void Update();
}
