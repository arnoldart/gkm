using UnityEngine;

public class MovementState : PlayerBaseState
{
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

        // float targetRunSpeed = stateMachine.currentSpeed > 5f ? 1f : 0f;

        float targetRunSpeed;

        if (stateMachine.currentSpeed >= stateMachine.slowRunSpeed &&
            stateMachine.currentSpeed <= stateMachine.runSpeed)
        {
            targetRunSpeed = 2f;
        }else if (stateMachine.currentSpeed <= stateMachine.slowRunSpeed &&
                  stateMachine.currentSpeed >= stateMachine.walkSpeed && stateMachine.walkScene == false)
        {
            targetRunSpeed = 1f;
        }
        else
        {
            targetRunSpeed = 0f;
        }

        // switch (stateMachine.currentSpeed)
        // {
        //     case float speed when speed >= stateMachine.runSpeed && stateMachine.isRunning:
        //         targetRunSpeed = 2f;
        //         break;
        //     case float speed when speed >= stateMachine.slowRunSpeed :
        //         targetRunSpeed = 1f;
        //         break;
        //     case float speed when speed <= stateMachine.walkSpeed && !stateMachine.walkScene:
        //         targetRunSpeed = 0f;
        //         break;
        //     default:
        //         targetRunSpeed = 1f;
        //         break;
        // }
        
        //
        //
        // if (stateMachine.currentSpeed >= stateMachine.runSpeed)
        // {
        //     targetRunSpeed = 1f;
        // }
        // else if (stateMachine.currentSpeed >= stateMachine.slowRunSpeed)
        // {
        //     targetRunSpeed = 2f;
        // }
        // else
        // {
        //     targetRunSpeed = 1f;
        // }
        
        float currentRunSpeed = stateMachine.Animator.GetFloat("runspeed");
        stateMachine.Animator.SetFloat("runspeed", Mathf.Lerp(currentRunSpeed, targetRunSpeed, Time.deltaTime * stateMachine.acceleration));
        
        // if (stateMachine.currentSpeed > 5f)
        // {
        //     stateMachine.Animator.SetFloat("runspeed", 1);
        // }
        // else
        // {
        //     stateMachine.Animator.SetFloat("runspeed", 0);
        // }
        
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

        if (!stateMachine.controller.isGrounded)
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
}
