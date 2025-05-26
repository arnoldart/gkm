using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookAIM : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Virtual Camera yang akan di-zoom saat aim")]
    public CinemachineFreeLook freeLookCamera;

    [Tooltip("Reference ke Player StateMachine untuk mendapatkan input AIM")]
    public PlayerStateMachine playerStateMachine;

    [Header("FOV Settings")]
    [Tooltip("Field of View normal (default)")]
    public float normalFOV = 40f;

    [Tooltip("Field of View saat AIM")]
    public float aimFOV = 28f;

    [Tooltip("Kecepatan transisi FOV")]
    public float fovTransitionSpeed = 10f;

    [Header("Tracked Object Offset")]
    [Tooltip("Offset normal saat tidak aim")]
    public Vector3 normalOffset;

    [Tooltip("Offset saat AIM")]
    public Vector3 aimTrackedOffset;

    [Header("Direct Input (Fallback)")]
    [Tooltip("Gunakan input manual jika tidak ada PlayerStateMachine")]
    public bool useDirectInput = false;

    private float targetFOV;
    private Vector3 targetOffset;
    private PlayerInputActions playerInputActions;
    private bool aimPressed = false;

    void Start()
    {
        if (freeLookCamera == null)
            freeLookCamera = GetComponent<CinemachineFreeLook>();

        // Setup fallback input jika tidak ada PlayerStateMachine
        if (playerStateMachine == null || useDirectInput)
        {
            useDirectInput = true;
            playerInputActions = new PlayerInputActions();
            playerInputActions.WeaponShortcut.Enable();
            playerInputActions.WeaponShortcut.AIM.performed += OnAimPerformed;
            playerInputActions.WeaponShortcut.AIM.canceled += OnAimCanceled;
        }

        targetFOV = normalFOV;
        targetOffset = normalOffset;
        freeLookCamera.m_Lens.FieldOfView = normalFOV;
        for (int i = 0; i < 3; i++)
        {
            var composer = freeLookCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            composer.m_TrackedObjectOffset = normalOffset;
        }
    }

    void Update()
    {
        bool isAiming = false;

        if (useDirectInput)
        {
            isAiming = aimPressed;
        }
        else if (playerStateMachine != null && playerStateMachine.InputHandler != null)
        {
            isAiming = playerStateMachine.InputHandler.AimPressed;
        }

        targetFOV = isAiming ? aimFOV : normalFOV;
        targetOffset = isAiming ? aimTrackedOffset : normalOffset;

        if (freeLookCamera.m_Lens.FieldOfView != targetFOV)
        {
            freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(
                freeLookCamera.m_Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * fovTransitionSpeed
            );
        }
        for (int i = 0; i < 3; i++)
        {
            var composer = freeLookCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
            if (composer != null)
            {
                composer.m_TrackedObjectOffset = Vector3.Lerp(
                    composer.m_TrackedObjectOffset,
                    targetOffset,
                    Time.deltaTime * fovTransitionSpeed
                );
            }
        }
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        aimPressed = true;
        Debug.Log("FreeLookAIM: Aim button pressed");
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        aimPressed = false;
        Debug.Log("FreeLookAIM: Aim button released");
    }

    private void OnDestroy()
    {
        if (useDirectInput && playerInputActions != null)
        {
            playerInputActions.WeaponShortcut.AIM.performed -= OnAimPerformed;
            playerInputActions.WeaponShortcut.AIM.canceled -= OnAimCanceled;
            playerInputActions.WeaponShortcut.Disable();
            playerInputActions.Dispose();
        }
    }
}
