using UnityEngine;

public class MovementState : PlayerBaseState
{

    private float groundedGraceTime = .2f;
    private float lastGroundedTime;

    public MovementState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.SetBool("isWalk", true);
        stateMachine.Animator.SetFloat("runspeed", 0);
        stateMachine.currentSpeed = 0;
        
        base.Enter();
    }

    public override void UpdateLogic()
    {
        Vector3 movement = GetCameraAdjustedMovement();
        float targetSpeed = stateMachine.isRunning ? stateMachine.runSpeed : GetTargetSpeed();
        stateMachine.currentSpeed = Mathf.Lerp(stateMachine.currentSpeed, targetSpeed, Time.deltaTime * stateMachine.acceleration);

        float targetRunSpeed = CalculateTargetRunSpeed();
        
        float currentRunSpeed = stateMachine.Animator.GetFloat("runspeed");
        stateMachine.Animator.SetFloat("runspeed", Mathf.Lerp(currentRunSpeed, targetRunSpeed, Time.deltaTime * stateMachine.acceleration));
        
        ApplyGravity();
        MoveCharacter(movement, stateMachine.currentSpeed);

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

        bool isEffectivelyGrounded = stateMachine.controller.isGrounded || (Time.time - lastGroundedTime < groundedGraceTime);
        bool isInFreeFall = stateMachine.verticalVelocity < -10f;

        if (!isEffectivelyGrounded && isInFreeFall)
        {
            stateMachine.ChangeState(new FallingState(stateMachine));
        }
    }

    public float GetTargetSpeed()
    {
        if (stateMachine.walkScene)
        {
            return stateMachine.walkSpeed;
        }
        else
        {
            return stateMachine.slowRunSpeed;
        }
    }
    
    private float CalculateTargetRunSpeed()
    {
        if (stateMachine.isRunning && !stateMachine.walkScene)
        {
            return 2f;
        }
        else if (stateMachine.currentSpeed <= stateMachine.slowRunSpeed &&
                 stateMachine.currentSpeed >= stateMachine.walkSpeed && !stateMachine.walkScene)
        {
            return 1f;
        }
        else
        {
            return stateMachine.walkScene ? 0f : 1f;
        }
    }
}
