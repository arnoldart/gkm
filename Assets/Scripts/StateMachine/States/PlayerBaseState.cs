using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected Vector3 GetCameraAdjustedMovement()
    {
        // Dapatkan vektor arah kamera
        Vector3 cameraForward = stateMachine.playerCamera.transform.forward;
        Vector3 cameraRight = stateMachine.playerCamera.transform.right;
        
        // Hilangkan komponen Y
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Hitung arah gerak relatif terhadap kamera
        Vector3 inputDirection = stateMachine.movementInput.normalized;
        return (cameraRight * inputDirection.x) + (cameraForward * inputDirection.z);
    }

    protected void ApplyGravity()
    {
        if (stateMachine.controller.isGrounded && stateMachine.verticalVelocity < 0)
        {
            stateMachine.verticalVelocity = -2f;
        }
        else
        {
            stateMachine.verticalVelocity += stateMachine.gravity * Time.deltaTime;
        }
    }

    protected void MoveCharacter(Vector3 motion, float speed)
    {
        Vector3 movement = motion * speed;
        movement.y = stateMachine.verticalVelocity;
        stateMachine.controller.Move(movement * Time.deltaTime);
    }
}