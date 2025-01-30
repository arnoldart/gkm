using UnityEngine;

public class IdleState : PlayerBaseState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();

        stateMachine.verticalVelocity = -2f;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        ApplyGravity();
        MoveCharacter(Vector3.zero, 0);

        // Check transitions
        if (stateMachine.movementInput != Vector3.zero)
        {
            stateMachine.ChangeState(new MovementState(stateMachine));
            return;
        }

        if (stateMachine.jumpTriggered && stateMachine.controller.isGrounded)
        {
            stateMachine.ChangeState(new JumpState(stateMachine));
            stateMachine.jumpTriggered = false;
            return;
        }

        if (!stateMachine.controller.isGrounded)
        {
            stateMachine.ChangeState(new FallingState(stateMachine));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
