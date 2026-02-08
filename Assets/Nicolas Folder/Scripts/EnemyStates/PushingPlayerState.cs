using KinematicCharacterController;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class PushingPlayerState : AEnemyState
{

    public PushingPlayerState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
    }

    public override void Enter()
    {
        enemyController.agent.speed = enemyData.pushingPlayerSpeed;
        PushPlayerToStart();
        Player.Instance.PlayRandomCaughtClip();

    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (enemyController.HasReachedDestination(enemyController.agent))
        {
            Debug.Log("Agent successfully brought player back to start position");
            GameStateManager.Instance.ChangeState(EGameState.GameOver);
        }
    }

    void PushPlayerToStart()
    {
        Debug.Log("Going to push player to start");
        // Attach player to agent
        Transform character = Player.Instance.KinematicCarController.transform;
        character.SetParent(enemyController.playerAttachPoint);
        character.localPosition = Vector3.zero;
        character.localRotation = Quaternion.identity;
        // Disable player's input
        Player.Instance.KinematicCarController.enabled = false;
        Player.Instance.KinematicCarController.GetComponent<KinematicCharacterMotor>().enabled = false;
        // Set destination to player's starting position
        enemyController.agent.SetDestination(PlayerStartPosition.Instance.Position);
    }
}
