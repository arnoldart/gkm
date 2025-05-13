using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// State untuk ketika pemain sedang membidik.
/// </summary>
public class AimState : PlayerBaseState
{
    private bool isAiming = true;
    
    // Parameters for mouse control
    private float mouseSensitivity = 2.0f;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    private float verticalLookMin = -30f;
    private float verticalLookMax = 60f;
    
    /// <summary>
    /// Membuat state Aim baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public AimState(PlayerStateMachine stateMachine) : base(stateMachine) 
    {
        // Store initial rotation when entering aim state
        if (PlayerStateMachine.PlayerCamera != null)
        {
            horizontalRotation = PlayerStateMachine.transform.rotation.eulerAngles.y;
            verticalRotation = PlayerStateMachine.PlayerCamera.transform.localRotation.eulerAngles.x;
            // Normalize vertical rotation to -180 to 180 range
            if (verticalRotation > 180f)
                verticalRotation -= 360f;
        }
    }

    public override void Enter()
    {
        // Ganti SetBool dengan CrossFadeInFixedTime
        PlayAnimation("Pistol Aim", 0.2f);
        PlayerStateMachine.IsAiming = true;
        
        // Lock cursor to center of screen when aiming
        // Cursor.lockState = CursorLockMode.Locked;

        // Activate Aim Rig layer
        var rigBuilder = PlayerStateMachine.GetComponent<RigBuilder>();
        if (rigBuilder != null)
        {
            foreach (var layer in rigBuilder.layers)
            {
                if (layer.rig.name == "Aim Rig")
                {
                    layer.active = true;
                    break;
                }
            }
        }
    }

    public override void UpdateLogic()
    {
        // Terapkan gravitasi
        ApplyGravity();
        
        // Update mouse look
        // HandleMouseLook();
        
        // Gerakan terbatas saat membidik
        Vector3 movement = GetCameraAdjustedMovement() * 0.5f; // Gerakan lebih lambat saat membidik
        MoveCharacter(movement, PlayerStateMachine.WalkSpeed);
        
        // Karakter selalu menghadap ke arah kamera saat membidik
        AlignWithCamera();
        
        // Periksa apakah pemain masih ingin membidik
        if (!PlayerStateMachine.IsAiming)
        {
            // Transisi ke state PistolAimToIdleState
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            return;
        }
        
        // Tangani transisi ke state lain
        if (PlayerStateMachine.JumpTriggered && PlayerStateMachine.Controller.isGrounded)
        {
            PlayerStateMachine.ChangeState(new JumpState(PlayerStateMachine));
            PlayerStateMachine.ConsumeJumpTrigger();
            return;
        }
        
        if (!PlayerStateMachine.Controller.isGrounded)
        {
            PlayerStateMachine.ChangeState(new FallingState(PlayerStateMachine));
        }
    }

    public override void Exit()
    {
        // Reset cursor state when exiting aim mode
        // Cursor.lockState = CursorLockMode.None;

        // Deactivate Aim Rig layer
        var rigBuilder = PlayerStateMachine.GetComponent<RigBuilder>();
        if (rigBuilder != null)
        {
            foreach (var layer in rigBuilder.layers)
            {
                if (layer.rig.name == "Aim Rig")
                {
                    layer.active = false;
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Menyelaraskan rotasi karakter dengan arah kamera.
    /// </summary>
    private void AlignWithCamera()
    {
        // Dapatkan arah forward kamera (tanpa komponen Y)
        Vector3 cameraForward = PlayerStateMachine.PlayerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        // Putar karakter untuk menghadap arah kamera
        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            PlayerStateMachine.transform.rotation = Quaternion.Slerp(
                PlayerStateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * 15f
            );
        }
    }
    
    /// <summary>
    /// Menangani kontrol kamera dengan mouse saat membidik.
    /// </summary>
    // private void HandleMouseLook()
    // {
    //     if (PlayerStateMachine.PlayerCamera == null) return;
        
    //     // Get mouse input
    //     float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    //     float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
    //     // Calculate rotation
    //     horizontalRotation += mouseX;
    //     verticalRotation -= mouseY; // Invert Y axis for intuitive control
    //     verticalRotation = Mathf.Clamp(verticalRotation, verticalLookMin, verticalLookMax);
        
    //     // Hapus pengaturan rotasi player di sini, biarkan AlignWithCamera() yang mengatur
    //     // Apply vertical rotation to the camera only
    //     PlayerStateMachine.PlayerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    // }
}