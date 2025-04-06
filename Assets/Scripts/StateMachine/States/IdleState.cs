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
        PlayerStateMachine.PlayerAnimator.SetBool("isWalk", false);
        PlayerStateMachine.VerticalVelocity = -2f;
    }

    public override void UpdateLogic()
    {
        ApplyGravity();
        MoveCharacter(Vector3.zero, 0);

        // Tangani transisi
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
        PlayerStateMachine.PlayerAnimator.SetBool("isWalk", true);
    }
}