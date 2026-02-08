using UnityEngine;
[System.Serializable]
public class AttackingState : AEnemyState
{
    public AttackingState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
    }

    public float grabDistance = 1f;
    public override void Enter()
    {
        Debug.Log("Agent enters attack state");
        enemyController.agent.speed = enemyData.runSpeed;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //Debug.Log("Agent is in attacking state...");
        UpdateAgentTarget();
        if (HasAgentReachedPlayer())
        {
            Debug.Log("Now grabbing player from AttackingState");
            // Switching state
            enemyStateManager.ChangeState(EEnemyState.PushingPlayer);
        }
    }
    void UpdateAgentTarget()
    {
        // Set agent's target to player position
        enemyController.agent.SetDestination(enemyVision.player.transform.position);
    }

    bool HasAgentReachedPlayer()
    {
        //if (enemyController.HasReachedDestination(enemyController.agent))
        //{
            float dist = Vector3.Distance(enemyController.gameObject.transform.position, enemyVision.player.transform.position);
            if (dist < grabDistance)
            {
                Debug.Log("Assez proche pour grab le joueur");
                return true;
            }

        //}
        return false;
    }
}
