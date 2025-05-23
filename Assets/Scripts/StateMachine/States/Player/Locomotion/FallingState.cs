using UnityEngine;

/// <summary>
/// State untuk ketika pemain jatuh (bergerak ke bawah).
/// </summary>
public class FallingState : PlayerBaseState
{
    /// <summary>
    /// Membuat state Jatuh baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public FallingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        // Ganti animasi trigger dengan CrossFadeInFixedTime
        PlayAnimation("Fall", 0.2f);
    }

    public override void UpdateLogic()
    {
        // Hitung gerakan dengan kontrol terbatas di udara
        Vector3 movement = GetCameraAdjustedMovement() * PlayerStateMachine.AirControl;
        ApplyGravity();
        MoveCharacter(movement, 1f);
        RotateTowardsMovementDirection(movement);

        // Periksa untuk mendarat
        if (PlayerStateMachine.Controller.isGrounded)
        {
            // Pilih state berikutnya berdasarkan input
            if (PlayerStateMachine.MovementInput != Vector3.zero)
            {
                PlayerStateMachine.ChangeState(new MovementState(PlayerStateMachine));
            }
            else
            {
                PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            }
        }
    }

    public override void Exit() { }
}