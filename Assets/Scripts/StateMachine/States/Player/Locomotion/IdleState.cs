using UnityEngine;

/// <summary>
/// State untuk ketika pemain diam (tidak bergerak).
/// </summary>
public class IdleState : PlayerBaseState
{
    /// <summary>
    /// Membuat state Idle baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        // Gunakan CrossFadeInFixedTime alih-alih SetBool
        PlayAnimation("Idle", 0.2f);
        
        PlayerStateMachine.VerticalVelocity = -2f;
    }

    public override void UpdateLogic()
    {
        ApplyGravity();
        MoveCharacter(Vector3.zero, 0);

        // Tangani transisi
        if (PlayerStateMachine.IsAiming)
        {
            PlayerStateMachine.ChangeState(new AimState(PlayerStateMachine));
            return;
        }
        if (PlayerStateMachine.MovementInput != Vector3.zero)
        {
            PlayerStateMachine.ChangeState(new MovementState(PlayerStateMachine));
            return;
        }

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
        // Tidak perlu melakukan reset parameter bool
        // StateEntry baru akan melakukan transisi dengan CrossFade
    }
}