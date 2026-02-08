using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public EnemyData enemyData;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public Transform playerAttachPoint;
    private EnemyVision vision;

    public AgentActions agentActions;
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
    public void FacePosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            agent.velocity = direction * agent.speed;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    void HuntPlayer()
    {

    }

    void DetectPlayer()
    {
        if (vision.CanSeePlayer())
        {
            // Change state to warned
            // Start timer for warning duration

        }
    }
}
