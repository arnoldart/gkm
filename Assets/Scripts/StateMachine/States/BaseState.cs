using UnityEngine;

public class BaseState : State
{
    protected PlayerController stateMachine;
    
    public BaseState(PlayerController stateMachine) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }
}
