using UnityEngine;

/// <summary>
/// State untuk transisi dari membidik kembali ke idle.
/// </summary>
public class PistolAimToIdleState : PlayerBaseState
{
    private float transitionTimer = 0f;
    private float transitionDuration = 0.5f; // Durasi animasi transisi
    
    /// <summary>
    /// Membuat state transisi dari membidik ke idle baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public PistolAimToIdleState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        // Play animation transition
        PlayAnimation("Pistol Aim to idle", 0.1f);
        
        // Reset timer
        transitionTimer = 0f;
        
        // Allow mouse cursor to be visible again
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void UpdateLogic()
    {
        // Terapkan gravitasi
        ApplyGravity();
        
        // Gerakan terbatas selama transisi
        Vector3 movement = GetCameraAdjustedMovement() * 0.7f; // Gerakan sedikit lebih cepat dari aiming tapi masih terbatas
        MoveCharacter(movement, PlayerStateMachine.WalkSpeed);
        
        // Karakter masih menghadap ke arah kamera selama transisi
        AlignWithCamera();
        
        // Hitung waktu transisi
        transitionTimer += Time.deltaTime;
        
        // Setelah transisi selesai, kembali ke state yang sesuai
        if (transitionTimer >= transitionDuration)
        {
            // Jika ada input gerakan, pindah ke MovementState
            if (PlayerStateMachine.MovementInput != Vector3.zero)
            {
                PlayerStateMachine.ChangeState(new MovementState(PlayerStateMachine));
            }
            else
            {
                // Jika tidak ada input gerakan, kembali ke IdleState
                PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            }
        }
        
        // Jika pemain menekan tombol membidik lagi selama transisi, kembali ke AimState
        if (PlayerStateMachine.IsAiming)
        {
            PlayerStateMachine.ChangeState(new AimState(PlayerStateMachine));
        }
    }

    public override void Exit()
    {
        // Tidak diperlukan tindakan khusus saat keluar
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
}