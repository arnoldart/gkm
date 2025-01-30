using UnityEngine;

public class MovementState : PlayerBaseState
{
    public MovementState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void UpdateLogic()
    {
        Vector3 movement = GetCameraAdjustedMovement(); // Gunakan pergerakan relatif kamera
        float speed = stateMachine.isRunning ? stateMachine.runSpeed : stateMachine.walkSpeed;

        ApplyGravity();
        MoveCharacter(movement, speed);

        // Rotasi karakter sesuai arah gerak
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            stateMachine.transform.rotation = Quaternion.Slerp(
                stateMachine.transform.rotation,
                targetRotation,
                Time.deltaTime * 15f
            );
        }

        // Check transitions
        if (stateMachine.movementInput == Vector3.zero)
        {
            stateMachine.ChangeState(new IdleState(stateMachine));
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
}
