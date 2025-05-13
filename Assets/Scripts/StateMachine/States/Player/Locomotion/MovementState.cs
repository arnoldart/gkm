using UnityEngine;

public class MovementState : PlayerBaseState
{
    private float groundCheckBuffer = 0.2f;
    private float timeSinceGrounded = 0f;
    private float slopeLimit = 45f;

    public MovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        PlayerStateMachine.PlayerAnimator.SetBool("isWalk", true);
        PlayerStateMachine.PlayerAnimator.SetFloat("runspeed", 0);
        PlayerStateMachine.CurrentSpeed = 0;
    }

    public override void UpdateLogic()
    {
        Vector3 movement = GetCameraAdjustedMovement();
        
        float targetSpeed = CalculateSpeedTarget();
        PlayerStateMachine.CurrentSpeed = Mathf.Lerp(
            PlayerStateMachine.CurrentSpeed, 
            targetSpeed, 
            Time.deltaTime * PlayerStateMachine.Acceleration
        );

        float targetRunSpeed = CalculateRunningSpeedTarget();
        float currentRunSpeed = PlayerStateMachine.PlayerAnimator.GetFloat("runspeed");
        PlayerStateMachine.PlayerAnimator.SetFloat(
            "runspeed", 
            Mathf.Lerp(currentRunSpeed, targetRunSpeed, Time.deltaTime * PlayerStateMachine.Acceleration)
        );
        
        ApplyGravity();
        MoveCharacter(movement, PlayerStateMachine.CurrentSpeed);
        RotateTowardsMovementDirection(movement);

        CheckGrounding();

        if (PlayerStateMachine.MovementInput == Vector3.zero)
        {
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            return;
        }

        if (PlayerStateMachine.JumpTriggered && IsGroundedWithBuffer())
        {
            // Ambil arah gerak horizontal (tanpa Y)
            Vector3 currentMovement = GetCameraAdjustedMovement();
            currentMovement.y = 0f;
            // Hitung kecepatan horizontal aktual (tanpa Y)
            float horizontalSpeed = PlayerStateMachine.CurrentSpeed;
            // Momentum hanya pada sumbu XZ, tidak Y
            PlayerStateMachine.JumpMomentum = currentMovement.normalized * horizontalSpeed;
            Debug.Log($"Jump momentum: {PlayerStateMachine.JumpMomentum}");
            PlayerStateMachine.ChangeState(new JumpState(PlayerStateMachine));
            PlayerStateMachine.ConsumeJumpTrigger();
            return;
        }

        if (!IsGroundedWithBuffer() && PlayerStateMachine.VerticalVelocity < -1f)
        {
            PlayerStateMachine.ChangeState(new FallingState(PlayerStateMachine));
        }
    }

    public override void Exit()
    {
        PlayerStateMachine.PlayerAnimator.SetBool("isWalk", false);
    }

    private float CalculateSpeedTarget()
    {
        if (PlayerStateMachine.IsRunning)
        {
            return PlayerStateMachine.RunSpeed;
        }
        
        return PlayerStateMachine.WalkScene ? 
            PlayerStateMachine.WalkSpeed : 
            PlayerStateMachine.SlowRunSpeed;
    }
    
    
    private float CalculateRunningSpeedTarget()
    {
        if (PlayerStateMachine.IsRunning && !PlayerStateMachine.WalkScene)
        {
            return 2f;
        }
        else if (PlayerStateMachine.CurrentSpeed <= PlayerStateMachine.SlowRunSpeed &&
                 PlayerStateMachine.CurrentSpeed >= PlayerStateMachine.WalkSpeed && 
                 !PlayerStateMachine.WalkScene)
        {
            return 1f;
        }
        else
        {
            return PlayerStateMachine.WalkScene ? 0f : 1f;
        }
    }

    private void CheckGrounding()
    {
        if (PlayerStateMachine.Controller.isGrounded)
        {
            timeSinceGrounded = 0f;
        }
        else
        {
            timeSinceGrounded += Time.deltaTime;
            
            if (IsOnSlope())
            {
                timeSinceGrounded = 0f;
            }
        }
    }

    private bool IsGroundedWithBuffer()
    {
        return timeSinceGrounded < groundCheckBuffer;
    }

    private bool IsOnSlope()
    {
        Ray ray = new Ray(PlayerStateMachine.Controller.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.1f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle < slopeLimit;
        }
        return false;
    }
}