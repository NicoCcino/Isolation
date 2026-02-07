using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Movement")]

    public float speed;
    public float detectionSpeed;

    [Header("Vision")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float perceiveRadius = 2f;

}
