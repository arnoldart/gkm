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

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            _stateMachine.ChangeState(new WalkState(_stateMachine));
        }
    }
}
