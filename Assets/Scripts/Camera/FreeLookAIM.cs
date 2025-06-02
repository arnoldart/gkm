using Cinemachine;
using UnityEngine;

public class FreeLookAIM : MonoBehaviour
{

    [Header("References")]
    [Tooltip("Virtual Camera yang akan di-zoom saat aim")]
    public CinemachineFreeLook freeLookCamera;
    [Tooltip("Script InputHandler yang akan dibaca status AimPressed-nya")]
    public InputHandler inputHandler;

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

    private float targetFOV;
    private Vector3 targetOffset;


    void Start()
    {
        if (freeLookCamera == null)
            freeLookCamera = GetComponent<CinemachineFreeLook>();
        if (inputHandler == null)
            inputHandler = GetComponent<InputHandler>();
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
        if (inputHandler != null)
        {
            isAiming = inputHandler.AimPressed;
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
}
