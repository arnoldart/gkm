using UnityEngine;

/// <summary>
/// State untuk ketika pemain melompat (bergerak ke atas).
/// </summary>
public class JumpState : PlayerBaseState
{
    /// <summary>
    /// Membuat state Lompat baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        // Tetapkan kecepatan vertikal awal
        PlayerStateMachine.VerticalVelocity = PlayerStateMachine.JumpForce;
        
        // Ganti SetTrigger dengan CrossFadeInFixedTime
        PlayAnimation("Jump", 0.1f);
    }

    public override void UpdateLogic()
    {
        // Hitung gerakan dengan kontrol terbatas di udara
        Vector3 movement = GetCameraAdjustedMovement() * PlayerStateMachine.AirControl;
        ApplyGravity();
        MoveCharacter(movement, 1f);
        RotateTowardsMovementDirection(movement);

        // Transisi ke jatuh ketika mencapai puncak
        if (PlayerStateMachine.VerticalVelocity <= 0)
        {
            PlayerStateMachine.ChangeState(new FallingState(PlayerStateMachine));
            return;
        }

        // Transisi ke idle jika entah bagaimana menyentuh tanah
        if (PlayerStateMachine.Controller.isGrounded)
        {
            PlayerStateMachine.ConsumeJumpTrigger();
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
        }
    }

    public override void Exit() { }
}