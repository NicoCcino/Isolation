using UnityEngine;
using KinematicCharacterController;
using UnityEngine.InputSystem;
using System;
public struct CarInputs
{
    public float MoveAxisForward; // +1 for Forward (Z/W), -1 for Backward (S)
    public float MoveAxisSteering; // -1 for Left (Q/A), +1 for Right (D)
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class KinematicCarController : MonoBehaviour, ICharacterController
{
    [Header("References")]
    public KinematicCharacterMotor Motor;
    public Transform CameraFollowPoint;


    [Header("Input Settings")]
    [SerializeField] private InputActionReference moveInputReference;

    [Header("Movement Settings")]
    [Tooltip("Maximum forward speed in meters/second")]
    public float MaxSpeed = 30f;
    [Tooltip("Maximum reverse speed in meters/second")]
    public float MaxReverseSpeed = 10f;
    [Tooltip("How fast the car accelerates")]
    public float Acceleration = 15f;
    [Tooltip("How fast the car decelerates when letting go of gas")]
    public float Deceleration = 10f;
    [Tooltip("Braking force when pressing opposite direction")]
    public float BrakeForce = 25f;
    [Tooltip("Gravity force applied to the car")]
    public float Gravity = 30f;
    [Tooltip("How much speed is lost while in the air")]
    public float AirDrag = 0.1f;

    [Header("Steering Settings")]
    [Tooltip("Degrees per second the car turns")]
    public float SteeringSpeed = 100f;
    [Tooltip("Minimum speed required to steer fully (prevents spinning in place)")]
    public float MinSpeedForSteering = 0.5f;
    [Tooltip("How much velocity is preserved during a turn (0 = drift/slide, 1 = full grip)")]
    [Range(0f, 1f)]
    public float TireGrip = 0.95f;

    [Header("Audio")]
    [SerializeField] private AudioEventIntermediary gruntAudioEventIntermediary;
    [SerializeField] private AudioEventIntermediary hitAudioEventIntermediary;
    [SerializeField] private AudioLoopBlender audioLoopBlender;


    private float timeAccelerating = 0f;
    private Vector3 _moveInputVector;
    private CarInputs _inputs;
    public Action OnHit;


    private void Awake()
    {
        // Assign the motor and link the controller
        Motor = GetComponent<KinematicCharacterMotor>();
        Motor.CharacterController = this;
    }

    private void Update()
    {
        HandleInputs();
        audioLoopBlender.blendValue = Mathf.Clamp(timeAccelerating / 10f, 0, 1);
    }

    private void HandleInputs()
    {
        // Read New Input System
        if (moveInputReference != null && moveInputReference.action != null)
        {
            _moveInputVector = moveInputReference.action.ReadValue<Vector2>();

            // Construct inputs struct
            _inputs.MoveAxisForward = _moveInputVector.y;
            _inputs.MoveAxisSteering = _moveInputVector.x;
        }
    }

    /// <summary>
    /// (Motor Callback) Handles rotation logic. 
    /// Cars rotate based on steering input, but usually only when moving.
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_inputs.MoveAxisSteering != 0f)
        {
            // Optional: Only allow steering if the car is moving slightly to behave like a real car
            // Remove this check if you want arcade-style pivot turning
            float speedRatio = Mathf.Clamp01(Motor.BaseVelocity.magnitude / MinSpeedForSteering);

            // Calculate rotation
            Vector3 rotationAxis = Motor.CharacterUp;
            float rotationAmount = _inputs.MoveAxisSteering * SteeringSpeed * speedRatio * deltaTime;

            // Reverse steering logic when going backward? (Optional, most games keep it standard)
            // if (Vector3.Dot(Motor.BaseVelocity, Motor.CharacterForward) < -0.1f) rotationAmount *= -1f;

            Quaternion rotationDelta = Quaternion.Euler(rotationAxis * rotationAmount);
            currentRotation = currentRotation * rotationDelta;
        }
    }

    /// <summary>
    /// (Motor Callback) Handles velocity logic.
    /// Applies acceleration, gravity, and lateral tire friction.
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // 1. Separate Vertical (Gravity) and Horizontal (Movement) velocity
        Vector3 targetMovementVelocity = Vector3.zero;
        if (Motor.GroundingStatus.IsStableOnGround)
        {
            // Reorient velocity to align with the new ground slope
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

            // Calculate Forward/Right vectors relative to the car's rotation and ground
            Vector3 inputRight = Vector3.Cross(Motor.CharacterForward, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized;

            // Decompose velocity into Forward speed and Lateral (slide) speed
            float currentForwardSpeed = Vector3.Dot(currentVelocity, reorientedInput);
            Vector3 lateralVelocity = Vector3.Project(currentVelocity, inputRight);

            // --- TIRE GRIP (Lateral Friction) ---
            // Kill off lateral velocity based on TireGrip setting. 
            // 1.0 means the car goes exactly where it looks. 0.0 means it slides on ice.
            currentVelocity -= lateralVelocity * TireGrip;

            // --- ACCELERATION / BRAKING ---
            float targetSpeed = 0f;
            float accelRate = 0f;

            // Gas (Forward)
            if (_inputs.MoveAxisForward > 0f)
            {
                targetSpeed = MaxSpeed;
                accelRate = Acceleration;
                timeAccelerating = Mathf.Clamp(timeAccelerating + Time.deltaTime, 0, 10);

            }
            // Reverse / Brake
            else if (_inputs.MoveAxisForward < 0f)
            {
                // If moving forward, this is braking
                if (currentForwardSpeed > 0.1f)
                {
                    targetSpeed = 0f;
                    accelRate = BrakeForce;
                }
                // If stopped or moving back, this is reversing
                else
                {
                    targetSpeed = -MaxReverseSpeed;
                    accelRate = Acceleration;
                }
            }
            // Coasting (No input)
            else
            {
                targetSpeed = 0f;
                accelRate = Deceleration;
                timeAccelerating = Mathf.Clamp(timeAccelerating - Time.deltaTime, 0, 10);
            }

            // Apply Forward Acceleration
            // We calculate the difference between where we are and where we want to be
            float speedDiff = targetSpeed - currentForwardSpeed;

            // Apply the change, clamped by acceleration rate
            float movementChange = Mathf.Clamp(speedDiff, -accelRate * deltaTime, accelRate * deltaTime);

            // Add the force to the velocity
            currentVelocity += reorientedInput * movementChange;
        }
        else
        {
            // AIRBORNE STATE
            // Apply Gravity
            currentVelocity += Motor.CharacterUp * -Gravity * deltaTime;

            // Apply Air Drag
            currentVelocity *= (1f / (1f + (AirDrag * deltaTime)));
        }
    }

    // --- Required ICharacterController Boilerplate ---

    public void BeforeCharacterUpdate(float deltaTime) { }
    public void PostGroundingUpdate(float deltaTime) { }
    public void AfterCharacterUpdate(float deltaTime) { }
    public bool IsColliderValidForCollisions(Collider coll) { return true; }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        Debug.Log($"Player  collision with {hitCollider.gameObject.name}");
        if (hitCollider.tag == "DontTriggerSound") return;
        gruntAudioEventIntermediary.PlayAudioEvent();
        hitAudioEventIntermediary.PlayAudioEvent();
        OnHit?.Invoke();
    }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        Debug.Log($"Player discrete collision with {hitCollider.gameObject.name}");
    }
}