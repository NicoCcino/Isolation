using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform player;
    public EnemyData enemyData;
    public LayerMask obstacleMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            Debug.Log("Player spotted!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.viewDistance);

        Vector3 left = Quaternion.Euler(0, -enemyData.viewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, enemyData.viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, left * enemyData.viewDistance);
        Gizmos.DrawRay(transform.position, right * enemyData.viewDistance);
    }

    public bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // Distance check
        if (Vector3.Distance(transform.position, player.position) > enemyData.viewDistance)
            return false;

        // Angle check
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > enemyData.viewAngle / 2f)
            return false;

        // Line of sight
        if (Physics.Raycast(transform.position + Vector3.up * 1.6f,
                            dirToPlayer,
                            Vector3.Distance(transform.position, player.position),
                            obstacleMask))
            return false;

        return true;
    }
}
