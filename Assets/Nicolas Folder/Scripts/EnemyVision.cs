using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform player;
    public EnemyData enemyData;
    public LayerMask obstacleMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        player = Player.Instance.KinematicCarController.CameraFollowPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            Debug.Log("Player seen!");
        }
        if (CanPerceivePlayer())
        {
            Debug.Log("Player perceived!");
        }
    }

    void OnDrawGizmos()
    {
        if (enemyData == null) return;

        float halfAngle = enemyData.viewAngle / 2f;
        int segments = 20; // plus = cône plus lisse
        Vector3 origin = transform.position + Vector3.up * 1.6f;

        // --- Cone principal ---
        Gizmos.color = Color.yellow;
        Vector3 prevPoint = origin;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + (enemyData.viewAngle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 point = origin + dir * enemyData.viewDistance;

            Gizmos.DrawLine(origin, point);

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }

        // --- Cone secondaire (perceiveRadius) ---
        Gizmos.color = Color.green; // couleur différente
        prevPoint = origin;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + (enemyData.viewAngle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 point = origin + dir * enemyData.perceptionDistance;

            Gizmos.DrawLine(origin, point);

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }
    public bool IsPlayerInDetectionCone(float detectionDistance, float detectionAngle)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // Distance check
        if (Vector3.Distance(transform.position, player.position) > detectionDistance)
            return false;

        // Angle check
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > detectionAngle / 2f)
            return false;

        // Line of sight
        if (Physics.Raycast(transform.position + Vector3.up * 1.6f,
                            dirToPlayer,
                            Vector3.Distance(transform.position, player.position),
                            obstacleMask))
            return false;

        return true;
    }

    public bool CanSeePlayer()
    {
        if (IsPlayerInDetectionCone(enemyData.viewDistance, enemyData.viewAngle))
        {
            // TODO: Add a check if player is in light or shadows
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanPerceivePlayer()
    {
        return IsPlayerInDetectionCone(enemyData.perceptionDistance, enemyData.perceptionAngle);
    }
}
