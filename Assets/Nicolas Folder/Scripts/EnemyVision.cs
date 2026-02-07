using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform player;
    public EnemyData enemyData;
    public LayerMask obstacleMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Player.Instance.KinematicCarController.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            //Debug.Log("Player seen!");
        }
        if (CanPerceivePlayer())
        {
            //Debug.Log("Player perceived!");
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
        Vector3 origin = transform.position + Vector3.up * 1.6f;
        //Vector3 toPlayer = player.position - transform.position;
        Vector3 toPlayer = player.position - (origin+transform.position)/2;
        Vector3 dirToPlayer = toPlayer.normalized;
        float distanceToPlayer = toPlayer.magnitude;

        // --- Distance check ---
        if (distanceToPlayer > detectionDistance)
        {
            //Debug.Log("Player hors distance");
            Debug.DrawRay(origin, dirToPlayer * detectionDistance, Color.gray);
            return false;
        }

        // --- Angle check ---
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > detectionAngle / 2f)
        {
            //Debug.Log($"Player hors angle (angle = {angle:F1})");
            Debug.DrawRay(origin, dirToPlayer * distanceToPlayer, Color.yellow);
            return false;
        }

        // --- Line of sight ---
        RaycastHit hit;
        if (Physics.Raycast(origin, dirToPlayer, out hit, distanceToPlayer, obstacleMask))
        {
            Debug.Log($"Vue bloquée par : {hit.collider.name}");
            Debug.DrawRay(origin, dirToPlayer * hit.distance, Color.red);
            return false;
        }

        // --- SUCCESS ---
        //Debug.Log("Player détecté !");
        Debug.DrawRay(origin, dirToPlayer * distanceToPlayer, Color.green);
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
