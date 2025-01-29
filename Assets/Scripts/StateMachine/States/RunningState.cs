using UnityEngine;

public class RunningState : MovementState
{
    public RunningState(StateMachine stateMachine, Transform playerTransform, CharacterController characterController, Vector2 moveInput, MovementConfig config, Transform cameraTransform)
        : base(stateMachine, playerTransform, characterController, moveInput, config, cameraTransform)
    {
    }

    protected override float GetSpeed() => _config.runSpeed;
}