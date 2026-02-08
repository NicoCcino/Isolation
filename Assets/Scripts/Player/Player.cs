using FOW;
using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input System
using NaughtyAttributes;
public class Player : Singleton<Player>
{
    public KinematicCarController KinematicCarController;
    public CameraController CameraController;
    public bool DisableCameraControl;
    public UIDebugInputs UiDebugInputs;

    [Header("Input Action References")]
    public InputActionReference MoveInput;       // Value (Vector2) - WASD/Left Stick
    public InputActionReference LookInput;       // Value (Vector2) - Mouse Delta/Right Stick


    [Header("Fog of War")]
    public FogOfWarRevealer PlayerRevealer;
    public FogOfWarHider PlayerHider;
    public LightShaderLerp lightShaderLerp;
    [ReadOnly] public bool IsInLight;
    [ReadOnly] public bool IsMakingNoise;
    private float makingNoiseTime = 0.2f;
    public AudioClip[] caughtClips;
    public AudioSource audioSource;
    private void EnableInputs()
    {
        // Ideally, enable inputs when the script becomes active
        EnableInput(MoveInput);
        EnableInput(LookInput);
    }

    private void OnDisable()
    {
        DisableInput(MoveInput);
        DisableInput(LookInput);
        KinematicCarController.OnHit -= OnHitCallback;
    }

    // Helper to safely enable actions
    private void EnableInput(InputActionReference input)
    {
        if (input != null && input.action != null) input.action.Enable();
    }

    // Helper to safely disable actions
    private void DisableInput(InputActionReference input)
    {
        if (input != null && input.action != null) input.action.Disable();
    }

    private void OnEnable()
    {

        EnableInputs();
        Cursor.lockState = CursorLockMode.Locked;

        if (CameraController == null) return;
        // Tell camera to follow transform
        CameraController.SetFollowTransform(KinematicCarController.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        CameraController.IgnoredColliders.Clear();
        CameraController.IgnoredColliders.AddRange(KinematicCarController.GetComponentsInChildren<Collider>());
        KinematicCarController.OnHit += OnHitCallback;

    }

    private void OnHitCallback()
    {
        IsMakingNoise = true;
        Invoke("ResetIsMakingNoise", makingNoiseTime);
    }
    private void ResetIsMakingNoise()
    {
        IsMakingNoise = false;
    }
    private void LateUpdate()
    {
        // // Handle rotating the camera along with physics movers
        // if (CameraController.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
        // {
        //     CameraController.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CameraController.PlanarDirection;
        //     CameraController.PlanarDirection = Vector3.ProjectOnPlane(CameraController.PlanarDirection, Character.Motor.CharacterUp).normalized;
        // }
        SetIsInLight(PlayerHider.NumObservers > 1);
        HandleCameraInput();
        UiDebugInputs.UpdateWithMoveInput(MoveInput.action.ReadValue<Vector2>());


    }
    private void SetIsInLight(bool value)
    {
        if (IsInLight != value)
        {
            IsInLight = value;
            lightShaderLerp.SetState(IsInLight);
        }
    }
    private void HandleCameraInput()

    {
        if (CameraController == null) return;
        // Create the look input vector for the camera
        Vector2 lookInput = Vector2.zero;
        if (LookInput != null)
        {
            lookInput = LookInput.action.ReadValue<Vector2>();
        }

        // Note: In the new Input System, mouse delta values are often much higher (pixels) 
        // than the old Input.GetAxis (0-1 approx). You might need to adjust sensitivity 
        // on your CharacterCamera component.
        Vector3 lookInputVector = new Vector3(lookInput.x, lookInput.y, 0f);
        // Apply inputs to the camera
        CameraController.UpdateCamera(Time.deltaTime);

    }
    public void PlayRandomCaughtClip()
    {
        AudioClip randomClip = caughtClips[Random.Range(0, caughtClips.Length)];
        audioSource.PlayOneShot(randomClip, 0.6f);
    }
}
