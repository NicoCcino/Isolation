using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public EnemyData enemyData;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public float waitTime = 0f;
    public float waitTimer = 0f;
    public int currentWaypointIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.speed = enemyData.speed;
        StartPatrol(waypoints);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (IsPatrolAvailable(waypoints))
        {
            // Debug.Log ("Checking if agent reached destination...");
            if (HasReachedDestination(agent))
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
        if (currentWaypointIndex == waypoints.Length)
        {
            currentWaypointIndex = 0; // Reset to the first waypoint
        }
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        waitTime = waypoints[currentWaypointIndex].GetComponent<PatrolWaypoint>().waitTime; // Set wait time
    }

    public bool HasReachedDestination(NavMeshAgent agent)
    {
        if (agent.pathPending) return false;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                return true;
            Debug.Log("Reached destination");
        }

        return false;
    }

}
