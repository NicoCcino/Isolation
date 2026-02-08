using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentActions : MonoBehaviour
{
    public NavMeshAgent agent;
    public float turnSpeed = 120f; // Degrees per second

    public void RotateToFace(Vector3 targetPosition)
    {
        StartCoroutine(RotateOverTime(targetPosition));
    }

    private IEnumerator RotateOverTime(Vector3 targetPosition)
    {
        // Optional: Stop the agent from moving while turning
        agent.isStopped = true;

        // Disable automatic rotation so our code takes control
        agent.updateRotation = false;

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Keep flat

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Rotate until we are within a small angle of the target
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
                yield return null; // Wait for next frame
            }
        }

        // Re-enable automatic rotation
        agent.updateRotation = true;
        agent.isStopped = false;
    }
}