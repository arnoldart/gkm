using UnityEngine;

public class FallingState : PlayerBaseState
{
    public FallingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() { }

    public override void UpdateLogic()
    {
        Vector3 movement = GetCameraAdjustedMovement() * stateMachine.airControl;
        ApplyGravity();
        MoveCharacter(movement, 1);

        if (stateMachine.controller.isGrounded)
        {
            if (stateMachine.movementInput != Vector3.zero)
            {
                stateMachine.ChangeState(new MovementState(stateMachine));
            }
            else
            {
                stateMachine.ChangeState(new IdleState(stateMachine));
            }
        }
    }

    public override void Exit() { }
}