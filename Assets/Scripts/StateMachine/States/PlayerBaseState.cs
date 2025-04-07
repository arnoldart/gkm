using UnityEngine;

/// <summary>
/// Kelas dasar untuk semua state khusus pemain.
/// </summary>
public abstract class PlayerBaseState : State
{
    /// <summary>
    /// Referensi ke state machine pemain.
    /// </summary>
    protected PlayerStateMachine PlayerStateMachine { get; }

    /// <summary>
    /// Konstruktor untuk state pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain yang memiliki state ini.</param>
    protected PlayerBaseState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        PlayerStateMachine = stateMachine;
    }

    /// <summary>
    /// Menghitung arah gerakan yang disesuaikan dengan orientasi kamera.
    /// </summary>
    /// <returns>Vektor gerakan yang disesuaikan dengan orientasi kamera.</returns>
    protected Vector3 GetCameraAdjustedMovement()
    {
        // Dapatkan vektor arah kamera
        Vector3 cameraForward = PlayerStateMachine.PlayerCamera.transform.forward;
        Vector3 cameraRight = PlayerStateMachine.PlayerCamera.transform.right;
        
        // Hilangkan komponen vertikal
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Hitung arah gerakan relatif terhadap kamera
        Vector3 inputDirection = PlayerStateMachine.MovementInput.normalized;
        return (cameraRight * inputDirection.x) + (cameraForward * inputDirection.z);
    }

    /// <summary>
    /// Menerapkan gravitasi pada kecepatan vertikal pemain.
    /// </summary>
    protected void ApplyGravity()
    {
        if (PlayerStateMachine.Controller.isGrounded && PlayerStateMachine.VerticalVelocity < 0)
        {
            PlayerStateMachine.VerticalVelocity = -2f;
        }
        else
        {
            PlayerStateMachine.VerticalVelocity += PlayerStateMachine.Gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Menggerakkan karakter menggunakan CharacterController.
    /// </summary>
    /// <param name="motion">Arah untuk bergerak.</param>
    /// <param name="speed">Pengali kecepatan.</param>
    protected void MoveCharacter(Vector3 motion, float speed)
    {
        Vector3 movement = motion * speed;
        movement.y = PlayerStateMachine.VerticalVelocity;
        PlayerStateMachine.Controller.Move(movement * Time.deltaTime);
    }
    
    /// <summary>
    /// Memutar karakter untuk menghadap arah gerakan.
    /// </summary>
    /// <param name="movement">Arah gerakan.</param>
    /// <param name="rotationSpeed">Kecepatan rotasi.</param>
    protected void RotateTowardsMovementDirection(Vector3 movement, float rotationSpeed = 15f)
    {
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            PlayerStateMachine.transform.rotation = Quaternion.Slerp(
                PlayerStateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}