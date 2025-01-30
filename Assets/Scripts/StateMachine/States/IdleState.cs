using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerController stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateLogic()
    {
        if (stateMachine.InputReader.MoveInput.sqrMagnitude > 0.01f)
        {
            if (stateMachine.InputReader.IsRunning)
            {
                Debug.Log("Running");
                stateMachine.ChangeState(new RunningState(stateMachine));
            }
            else
            {
                Debug.Log("Walking");
                stateMachine.ChangeState(new WalkState(stateMachine));
            }
        }
    }
}
