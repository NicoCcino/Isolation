using UnityEngine;
[System.Serializable]
public class PatrollingState : AEnemyState
{

    public float waitTime = 0f;
    protected float waitTimer = 0f;
    protected int currentWaypointIndex = 0;

    public PatrollingState(EnemyController enemyController, EnemyVision enemyVision, EnemyData enemyData, EnemyStateManager enemyStateManager, float waitTime) : base(enemyController, enemyVision, enemyData, enemyStateManager)
    {
        this.waitTime = waitTime;
    }

    public override void Enter()
    {
        enemyController.agent.speed = enemyData.walkSpeed;
        //if (IsPatrolAvailable(enemyController.waypoints))
        //{
        Debug.Log("Going to waypoint 0");
        enemyController.agent.SetDestination(enemyController.waypoints[currentWaypointIndex].position);
        waitTime = enemyController.waypoints[currentWaypointIndex].GetComponent<PatrolWaypoint>().waitTime; // Set wait time  
        //}

    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //Debug.Log("Agent is in patrolling state...");
        CheckDetection();
        UpdatePatrolMovement();
    }

    public bool IsPatrolAvailable(Transform[] wps)
    {
        if (wps == null || wps.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void StartPatrol(Transform[] wps)
    {
        if (IsPatrolAvailable(wps))
        {
            GoToNextPoint();
        }
        else
        {
            Debug.Log("No waypoints provided for patrol.");
        }
    }

    void UpdatePatrolMovement()
    {
        if (IsPatrolAvailable(enemyController.waypoints))
        {
            // Debug.Log ("Checking if agent reached destination...");
            if (enemyController.HasReachedDestination(enemyController.agent))
            {
                waitTimer += Time.deltaTime;
                if (waitTimer > waitTime)
                {
                    GoToNextPoint();
                }
            }
        }
    }

    void CheckDetection()
    {
        if (enemyVision.CanSeePlayer() || enemyVision.CanPerceivePlayer() || enemyVision.CanHearPlayer())
        {
            // Change state to warned
            enemyStateManager.ChangeState(EEnemyState.Warned);
            Debug.Log("Agent detected player during patrol, changing state to warned.");
        }
    }

    void GoToNextPoint()
    {
        Debug.Log("Going to next waypoint");
        waitTimer = 0f;
        Debug.Log("increasing currentWayPointIndex from " + currentWaypointIndex + " to " + (currentWaypointIndex + 1));
        currentWaypointIndex++;
        if (currentWaypointIndex == enemyController.waypoints.Length)
        {
            Debug.Log("Reached max waypoints count, resetting currentWayPointIndex");
            currentWaypointIndex = 0; // Reset to the first waypoint
        }
        enemyController.agent.SetDestination(enemyController.waypoints[currentWaypointIndex].position);
        waitTime = enemyController.waypoints[currentWaypointIndex].GetComponent<PatrolWaypoint>().waitTime; // Set wait time
    }
}
