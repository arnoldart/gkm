using UnityEngine;

public class State
{
    protected StateMachine _stateMachine;

    public State(StateMachine stateMachine)
    {
        this._stateMachine = stateMachine;
    }
    
    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }
}
