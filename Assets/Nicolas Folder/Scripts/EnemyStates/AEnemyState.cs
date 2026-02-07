using UnityEngine;
public abstract class AEnemyState : BaseState
{

    [SerializeField] public EnemyController enemyController;
    [SerializeField] public EnemyVision enemyVision;
    [SerializeField] public EnemyData enemyData;



    public override abstract void Enter();
    public override abstract void Exit();
    public override abstract void Update();
}
