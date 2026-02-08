using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Framing")]
    public Vector2 FollowPointFraming = new Vector2(0f, 0f);
    public float FollowingSharpness = 10000f;

    [Header("Distance & Orientation")]
    public float DefaultDistance = 6f;
    public float DistanceMovementSharpness = 10f;
    [Range(-90f, 90f)]
    public float VerticalAngle = 20f;
    public float HorizontalAngle = 0f; // Set this to match your desired "compass" direction

    [Header("Obstruction")]
    public float ObstructionCheckRadius = 0.2f;
    public LayerMask ObstructionLayers = -1;
    public float ObstructionSharpness = 10000f;
    public List<Collider> IgnoredColliders = new List<Collider>();

    public Transform Transform { get; private set; }
    public Transform FollowTransform { get; private set; }

    private float _currentDistance;
    private Vector3 _currentFollowPosition;
    private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
    private const int MaxObstructions = 32;

    void Awake()
    {
        Transform = this.transform;
        _currentDistance = DefaultDistance;
    }

    public void SetFollowTransform(Transform t)
    {
        FollowTransform = t;
        if (FollowTransform)
        {
            _currentFollowPosition = FollowTransform.position;
        }
    }

    public void UpdateCamera(float deltaTime)
    {
        if (!FollowTransform) return;

        // 1. Static World Rotation (Ignoring Player Rotation)
        // This ensures the camera always views from the same world-space angle
        Quaternion targetRotation = Quaternion.Euler(VerticalAngle, HorizontalAngle, 0);
        Transform.rotation = targetRotation;

        // 2. Smoothly follow the target position
        _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

        // 3. Obstruction Logic
        float targetDist = DefaultDistance;
        RaycastHit closestHit = new RaycastHit { distance = Mathf.Infinity };

        int hitCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, targetDist, ObstructionLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            if (IgnoredColliders.Contains(_obstructions[i].collider)) continue;

            if (_obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
            {
                closestHit = _obstructions[i];
            }
        }

        float desiredDistance = (closestHit.distance < Mathf.Infinity) ? closestHit.distance : targetDist;
        _currentDistance = Mathf.Lerp(_currentDistance, desiredDistance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));

        // 4. Calculate Final Position based on Static Rotation
        Vector3 targetPosition = _currentFollowPosition - (Transform.forward * _currentDistance);

        // Apply framing offsets
        targetPosition += Transform.right * FollowPointFraming.x;
        targetPosition += Transform.up * FollowPointFraming.y;

        Transform.position = targetPosition;
    }
}