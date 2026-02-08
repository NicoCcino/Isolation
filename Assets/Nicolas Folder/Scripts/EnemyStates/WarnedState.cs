using UnityEngine;
using NaughtyAttributes;
[System.Serializable]
public class WarnedState : AEnemyState
{
    protected float warnedTimer = 0f;
    public float delayBeforeMovingTowardsPlayer = 2f;
    protected float movingTowardsPlayerTimer = 0.0f;
    public float startWarnedTimer = 2f;
    public float warnedTimeToAttack = 4f;
    public GameObject exclamationMarkGO;
    protected EnemyAudio enemyAudio;

    public float WarnedTimer { get => warnedTimer; }

    public WarnedState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager, EnemyAudio enemyAudio, float warnedTime, GameObject exclamationMarkGO) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
        this.warnedTimeToAttack = warnedTime;
        this.exclamationMarkGO = exclamationMarkGO;
        this.enemyAudio = enemyAudio;
    }

    public override void Enter()
    {
        // Stop movement
        enemyController.agent.speed = 0;
        warnedTimer = startWarnedTimer;
        movingTowardsPlayerTimer = 0f;
        enemyController.agentActions.RotateToFace(Player.Instance.KinematicCarController.CameraFollowPoint.position);
        // Display exclamation mark
        exclamationMarkGO.SetActive(true);
        // TODO: Add jump action on NPC
        // Play sound
        enemyAudio.PlayWarnedSound();


    }

    public override void Exit()
    {
        // Hide exclamation mark
        exclamationMarkGO.SetActive(false);
    }

    public override void Update()
    {
        Debug.Log("Agent is in warned state...");

        if (movingTowardsPlayerTimer < delayBeforeMovingTowardsPlayer)
        {
            movingTowardsPlayerTimer += Time.deltaTime;
        }
        else
        {
            if (enemyController.agent.hasPath == false)
            {
                enemyController.agent.speed = 1;
                enemyController.agent.SetDestination(Player.Instance.transform.position);
            }
        }

        if (enemyVision.CanSeePlayer() || enemyVision.CanPerceivePlayer() || enemyVision.CanHearPlayer())
        {
            if (warnedTimer < startWarnedTimer)
                warnedTimer = startWarnedTimer;
            warnedTimer += Time.deltaTime;
        }
        else
        {
            warnedTimer -= Time.deltaTime;
        }
        if (WarnedTimer >= warnedTimeToAttack)
        {
            // Enemy has been detecting player for long enough, switching to Attack State
            enemyStateManager.ChangeState(EEnemyState.Attacking);
        }
        if (WarnedTimer <= 0)
        {
            enemyStateManager.ChangeState(EEnemyState.Patrolling);
        }

    }
}
