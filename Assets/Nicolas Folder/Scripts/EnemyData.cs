using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Movement")]

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float detectionSpeed;

    [Header("Vision")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float hearingDistance = 5f;
    [Header("Perception")]
    [Tooltip("Perception based on short distance - not considering lighting")]

    public float perceptionDistance = 2f;
    public float perceptionAngle = 90f;

}
