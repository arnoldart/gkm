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
        if (stateMachine.playerCamera == null)
        {
            Debug.LogWarning("Player camera is null, using default forward/right vectors");
            return stateMachine.movementInput.normalized;
        }
        
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
            stateMachine.verticalVelocity = PlayerConstants.GROUND_SNAP_FORCE;
        }
        else
        {
            stateMachine.verticalVelocity += stateMachine.Gravity * Time.deltaTime;
        }
    }

    protected void MoveCharacter(Vector3 motion, float speed)
    {
        Vector3 movement = motion * speed;
        movement.y = stateMachine.verticalVelocity;
        stateMachine.controller.Move(movement * Time.deltaTime);
    }
    
    protected bool IsEffectivelyGrounded(float lastGroundedTime)
    {
        bool isGrounded = stateMachine.controller.isGrounded;
        bool wasRecentlyGrounded = (Time.time - lastGroundedTime) < PlayerConstants.GROUNDED_GRACE_TIME;
        
        // Opsional: tambahkan raycast untuk slope detection
        if (!isGrounded && !wasRecentlyGrounded) 
        {
            Ray ray = new Ray(stateMachine.transform.position + Vector3.up * 0.1f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 0.5f, stateMachine.groundLayer))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle < stateMachine.MaxSlopeAngle)
                {
                    return true;
                }
            }
        }
        
        return isGrounded || wasRecentlyGrounded;
    }
}