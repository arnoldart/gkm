using UnityEngine;

public class WallClimbingState : PlayerBaseState
{
    public WallClimbingState(PlayerStateMachine stateMachine)
        : base(stateMachine) { }

    public override void Enter()
    {
        PlayerStateMachine.enabled = false;
        PlayerStateMachine.SetGravityEnabled(false);
    }

    public override void UpdateLogic()
    {

    }

    public override void UpdatePhysics()
    {

    }


    public override void Exit()
    {

    }
    
}
