using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Oalidate()
    {
        
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;

        // Paramètres Animator
        animator.SetFloat("Speed", speed);
        //animator.SetBool("IsMoving", speed > 0.1f);
    }
}
