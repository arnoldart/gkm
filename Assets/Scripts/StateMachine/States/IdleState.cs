using UnityEngine;

public class IdleState : State
{
    // private Animator _animator;
    
    public IdleState(StateMachine stateMachine) : base(stateMachine)
    {
        // this._animator = animator;
    }

    public override void Enter()
    {
        base.Enter();
        // _animator.SetTrigger("Idle");
    }
}
