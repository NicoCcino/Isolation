using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input System

public class Player : MonoBehaviour
{
    public CharacterController Character;
    public CameraController CameraController;

    [Header("Input Action References")]
    public InputActionReference MoveInput;       // Value (Vector2) - WASD/Left Stick
    public InputActionReference LookInput;       // Value (Vector2) - Mouse Delta/Right Stick
    public InputActionReference ScrollInput;     // Value (Vector2) - Mouse Scroll
    public InputActionReference JumpInput;       // Button - Space/South Button
    public InputActionReference CrouchInput;     // Button - C/East Button
    public InputActionReference ToggleZoomInput; // Button - Right Mouse Click
    public InputActionReference LockCursorInput; // Button - Left Mouse Click
    public InputActionReference AttackInput;

    private void EnableInputs()
    {

        // Ideally, enable inputs when the script becomes active
        EnableInput(MoveInput);
        EnableInput(LookInput);
        EnableInput(ScrollInput);
        EnableInput(JumpInput);
        EnableInput(CrouchInput);
        EnableInput(ToggleZoomInput);
        EnableInput(LockCursorInput);
        EnableInput(AttackInput);
    }

    private void OnDisable()
    {
        DisableInput(MoveInput);
        DisableInput(LookInput);
        DisableInput(ScrollInput);
        DisableInput(JumpInput);
        DisableInput(CrouchInput);
        DisableInput(ToggleZoomInput);
        DisableInput(LockCursorInput);
        DisableInput(AttackInput);
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
        Character.Motor.enabled = true;
        Character.enabled = true;
        CameraController.gameObject.SetActive(true);

        EnableInputs();
        Cursor.lockState = CursorLockMode.Locked;

        if (CameraController == null) return;
        // Tell camera to follow transform
        CameraController.SetFollowTransform(Character.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        CameraController.IgnoredColliders.Clear();
        CameraController.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());

    }

    private void Update()
    {

        // Re-lock cursor if user clicks (Left Mouse Button equivalent)
        if (LockCursorInput != null && LockCursorInput.action.WasPressedThisFrame())
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCharacterInput();
    }

    private void LateUpdate()
    {
        // // Handle rotating the camera along with physics movers
        // if (CameraController.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
        // {
        //     CameraController.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CameraController.PlanarDirection;
        //     CameraController.PlanarDirection = Vector3.ProjectOnPlane(CameraController.PlanarDirection, Character.Motor.CharacterUp).normalized;
        // }
        HandleCameraInput();
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

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = 0f;
#if !UNITY_WEBGL
        if (ScrollInput != null)
        {
            // New Input System Scroll is usually a Vector2.y. 
            // We normalize it loosely to match old GetAxis behavior or use raw depending on camera script needs.
            // Standard mouse scroll wheel is often +/- 120 in new system.
            float rawScroll = ScrollInput.action.ReadValue<Vector2>().y;

            // Simple normalization to keep it consistent with old GetAxis behavior (approx -1 to 1)
            // If your camera zooms too fast, adjust this divisor.
            if (Mathf.Abs(rawScroll) > 0)
            {
                scrollInput = -Mathf.Sign(rawScroll);
            }
        }
#endif

        // Apply inputs to the camera
        CameraController.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level (Right Mouse Button equivalent)
        if (ToggleZoomInput != null && ToggleZoomInput.action.WasPressedThisFrame())
        {
            CameraController.TargetDistance = (CameraController.TargetDistance == 0f) ? CameraController.DefaultDistance : 0f;
        }
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        if (MoveInput != null)
        {
            Vector2 move = MoveInput.action.ReadValue<Vector2>();
            characterInputs.MoveAxisForward = move.y;
            characterInputs.MoveAxisRight = move.x;
        }

        if (CameraController != null)
            characterInputs.CameraRotation = CameraController.Transform.rotation;

        // Handle Jump
        if (JumpInput != null)
        {
            characterInputs.JumpDown = JumpInput.action.WasPressedThisFrame();
        }

        // Handle Crouch
        if (CrouchInput != null)
        {
            characterInputs.CrouchDown = CrouchInput.action.WasPressedThisFrame();
            characterInputs.CrouchUp = CrouchInput.action.WasReleasedThisFrame();
        }

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}
