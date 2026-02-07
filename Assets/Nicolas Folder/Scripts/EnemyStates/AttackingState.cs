using UnityEngine;
[System.Serializable]
public class AttackingState :  AEnemyState
{
    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        Debug.Log("Agent is in attacking state...");
    }
}
