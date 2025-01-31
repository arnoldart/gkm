using UnityEngine;

public class JumpState : PlayerBaseState
{
    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.verticalVelocity = stateMachine.jumpForce;
        
        stateMachine.Animator.SetTrigger("jump");
    }

    public override void UpdateLogic()
    {
        Vector3 movement = GetCameraAdjustedMovement() * stateMachine.airControl;
        ApplyGravity();
        MoveCharacter(movement, 1);

        if (stateMachine.verticalVelocity <= 0)
        {
            stateMachine.ChangeState(new FallingState(stateMachine));
            return;
        }

        if (stateMachine.controller.isGrounded)
        {
            stateMachine.jumpTriggered = false;
            stateMachine.ChangeState(new IdleState(stateMachine));
        }
    }

    public override void Exit() { }
}