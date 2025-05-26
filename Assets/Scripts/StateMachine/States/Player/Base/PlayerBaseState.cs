using UnityEngine;

/// <summary>
/// Kelas dasar untuk semua state pemain.
/// </summary>
public abstract class PlayerBaseState : State
{
    /// <summary>
    /// Referensi ke state machine pemain.
    /// </summary>
    protected PlayerStateMachine PlayerStateMachine { get; private set; }

    /// <summary>
    /// Konstruktor untuk PlayerBaseState.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    protected PlayerBaseState(PlayerStateMachine stateMachine)
        : base(stateMachine)
    {
        PlayerStateMachine = stateMachine;
    }

    /// <summary>
    /// Menerapkan gravitasi pada kecepatan vertikal.
    /// </summary>
    protected void ApplyGravity()
    {
        // Jika gravitasi dimatikan, jangan terapkan gravitasi
        if (!PlayerStateMachine.IsGravityEnabled)
        {
            return;
        }

        if (PlayerStateMachine.Controller.isGrounded && PlayerStateMachine.VerticalVelocity < 0)
        {
            // Nilai kecil negatif untuk menjaga karakter tetap menempel ke tanah
            PlayerStateMachine.VerticalVelocity = PlayerStateMachine.Gravity;
        }
        else
        {
            // Aplikasikan gravitasi menggunakan method GetCurrentGravity()
            PlayerStateMachine.VerticalVelocity +=
                PlayerStateMachine.GetCurrentGravity() * Time.deltaTime;
        }
    }

    /// <summary>
    /// Menggerakkan karakter pemain dengan input yang diberikan.
    /// </summary>
    /// <param name="movement">Vektor gerakan yang dinormalisasi.</param>
    /// <param name="speedMultiplier">Faktor kecepatan.</param>
    protected void MoveCharacter(Vector3 movement, float speedMultiplier)
    {
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Hitung kecepatan berjalan
        Vector3 moveVelocity = movement * speedMultiplier;

        // Tambahkan vektor vertikal (gravitasi/lompatan)
        moveVelocity.y = PlayerStateMachine.VerticalVelocity;

        // Gerakkan karakter
        PlayerStateMachine.Controller.Move(moveVelocity * Time.deltaTime);
    }

    protected void MoveCharacterWithVector2(Vector2 movement, float speedMultiplier)
    {
        if (movement.magnitude == 0)
        {
            return;
        }

        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Hitung kecepatan berjalan
        Vector3 moveVelocity = new Vector3(movement.x, 0, movement.y) * speedMultiplier;

        // Tambahkan vektor vertikal (gravitasi/lompatan)
        moveVelocity.y = PlayerStateMachine.VerticalVelocity;

        // Gerakkan karakter
        PlayerStateMachine.Controller.Move(moveVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Mendapatkan vektor gerakan yang disesuaikan dengan orientasi kamera.
    /// </summary>
    /// <returns>Vektor gerakan yang disesuaikan dengan kamera.</returns>
    protected Vector3 GetCameraAdjustedMovement()
    {
        if (PlayerStateMachine.MovementInput == Vector3.zero)
        {
            return Vector3.zero;
        }

        // Dapatkan transformasi kamera
        Transform cameraTransform = PlayerStateMachine.PlayerCamera.transform;

        // Buat bidang XZ dari vektor forward dan right kamera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Hitung vektor gerakan relatif terhadap kamera
        return (
            forward * PlayerStateMachine.MovementInput.z
            + right * PlayerStateMachine.MovementInput.x
        );
    }

    /// <summary>
    /// Memutar karakter menghadap arah gerakan.
    /// </summary>
    /// <param name="movement">Vektor gerakan.</param>
    protected void RotateTowardsMovementDirection(Vector3 movement)
    {
        if (movement == Vector3.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(movement);
        PlayerStateMachine.transform.rotation = Quaternion.Slerp(
            PlayerStateMachine.transform.rotation,
            targetRotation,
            Time.deltaTime * 10f
        );
    }

    /// <summary>
    /// Memainkan animasi dengan crossfade untuk transisi yang mulus
    /// </summary>
    /// <param name="animationName">Nama animasi/state di animator</param>
    /// <param name="transitionDuration">Durasi transisi dalam detik</param>
    /// <param name="layer">Layer animator yang digunakan (default = 0)</param>
    /// <param name="normalizedTime">Waktu normalisasi untuk memulai animasi (default = 0)</param>
    protected void PlayAnimation(
        string animationName,
        float transitionDuration = 0.1f,
        int layer = 0,
        float normalizedTime = 0f
    )
    {
        PlayerStateMachine.PlayerAnimator.CrossFadeInFixedTime(
            animationName,
            transitionDuration,
            layer,
            normalizedTime
        );
    }

    public bool CheckForClimbableSurface()
    {
        RaycastHit hit;
        Vector3 rayOrigin = PlayerStateMachine.transform.position + Vector3.up * (PlayerStateMachine.Controller.height * 0.5f);

        // Debug raycast visualization
        // Debug.DrawRay(rayOrigin, transform.forward * climbDistance, Color.yellow, 0.5f);

        // Check if there's a climbable surface in front and player is pressing climb
        if (
            Physics.Raycast(
                rayOrigin,
                PlayerStateMachine.transform.forward,
                out hit,
                PlayerStateMachine.climbDistance,
                PlayerStateMachine.climbableLayerMask
            )
        )
        {
            // Draw a green line to the hit point for debug
            // Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.5f);

            if (PlayerStateMachine.InputHandler != null && PlayerStateMachine.InputHandler.IsClimbPressed())
            {
                PlayerStateMachine.InputHandler.ResetClimbPressed();
                return true;
            }
        }

        return false;
    }
}
