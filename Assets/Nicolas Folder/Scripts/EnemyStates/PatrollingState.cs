using UnityEngine;
[System.Serializable]
public class PatrollingState : AEnemyState
{

    public float waitTime = 0f;
    public float waitTimer = 0f;
    public int currentWaypointIndex = 0;
    public override void Enter()
    {
        StartPatrol(enemyController.waypoints);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        Debug.Log("Agent is in patrolling state...");
        UpdatePatrol();
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

    void UpdatePatrol()
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

    void GoToNextPoint()
    {
        Debug.Log("Going to next waypoint");
        waitTimer = 0f;
        currentWaypointIndex++;
        if (currentWaypointIndex == enemyController.waypoints.Length)
        {
            currentWaypointIndex = 0; // Reset to the first waypoint
        }
        enemyController.agent.SetDestination(enemyController.waypoints[currentWaypointIndex].position);
        waitTime = enemyController.waypoints[currentWaypointIndex].GetComponent<PatrolWaypoint>().waitTime; // Set wait time
    }
}
