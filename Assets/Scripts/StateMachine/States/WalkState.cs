using UnityEngine;

public class WalkState : BaseState
{
    public WalkState(PlayerController stateMachine) : base(stateMachine) {}

    public override void UpdatePhysics()
    {
        Vector3 direction = CalculateMovementDirection();
        Move(direction * stateMachine.MovementConfig.walkSpeed, Time.deltaTime);
        RotatePlayer(direction);
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 cameraForward = stateMachine.CameraTransform.forward;
        Vector3 cameraRight = stateMachine.CameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        return (cameraRight * stateMachine.InputReader.MoveInput.x + cameraForward * stateMachine.InputReader.MoveInput.y).normalized;
    }
    
    private void RotatePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        stateMachine.transform.rotation = Quaternion.Slerp(
            stateMachine.transform.rotation, 
            targetRotation, 
            stateMachine.MovementConfig.rotationSpeed * Time.deltaTime
        );
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        if (stateMachine.InputReader.MoveInput.sqrMagnitude > 0.01f)
        {
            Debug.Log($"State Decision - IsRunning: {stateMachine.InputReader.IsRunning}");

            if (stateMachine.InputReader.IsRunning)
            {
                Debug.Log("Switching to RunningState");
                stateMachine.ChangeState(new RunningState(stateMachine));
            }
            else
            {
                Debug.Log("Switching to WalkState");
                stateMachine.ChangeState(new WalkState(stateMachine));
            }
        }
    }
}