using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public EnemyData enemyData;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public Transform playerAttachPoint;
    private EnemyVision vision;

    void OnValidate()
    {
        if (vision == null)
        {
            vision = GetComponent<EnemyVision>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.speed = enemyData.walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void HuntPlayer()
    {
        
    }

    void DetectPlayer()
    {
        if (vision.CanSeePlayer()){
            // Change state to warned
            // Start timer for warning duration

        }
    }
}
