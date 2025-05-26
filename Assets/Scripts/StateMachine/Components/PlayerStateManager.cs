using UnityEngine;

/// <summary>
/// Komponen untuk mengelola state variables dan transisi untuk PlayerStateMachine.
/// </summary>
public class PlayerStateManager : MonoBehaviour
{
    [Header("State Variables")]
    public float CurrentSpeed { get; set; }
    public Vector3 MovementInput { get; set; }
    public float VerticalVelocity { get; set; }
    public bool IsRunning { get; set; }
    public bool JumpTriggered { get; set; }
    public bool IsAiming { get; set; }
    public bool IsHealing { get; set; }

    [Header("Jump Momentum")]
    public Vector3 JumpMomentum { get; set; }
    public float JumpMomentumMultiplier { get; set; } = 1.2f;

    /// <summary>
    /// Mengatur ulang pemicu lompat setelah dikonsumsi.
    /// </summary>
    public void ConsumeJumpTrigger()
    {
        JumpTriggered = false;
    }

    /// <summary>
    /// Memperbarui vektor input gerakan.
    /// </summary>
    /// <param name="input">Vektor input gerakan baru.</param>
    public void SetMovementInput(Vector3 input)
    {
        MovementInput = input;
    }

    /// <summary>
    /// Reset semua state ke nilai default.
    /// </summary>
    public void ResetStates()
    {
        CurrentSpeed = 0f;
        MovementInput = Vector3.zero;
        VerticalVelocity = 0f;
        IsRunning = false;
        JumpTriggered = false;
        IsAiming = false;
        IsHealing = false;
        JumpMomentum = Vector3.zero;
    }
}
