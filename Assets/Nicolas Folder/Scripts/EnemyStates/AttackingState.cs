using UnityEngine;
[System.Serializable]
public class AttackingState :  AEnemyState
{
    public override void Enter()
    {
        Debug.Log ("Agent enters attack state");
        enemyController.agent.speed = enemyData.runSpeed;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //Debug.Log("Agent is in attacking state...");
        UpdateAgentTarget();
    }
    void UpdateAgentTarget()
    {
        // Set agent's target to player position
        Debug.Log($"Agent is targetting pos {enemyVision.player.transform.position}");
        enemyController.agent.SetDestination(enemyVision.player.transform.position);
    }
}
