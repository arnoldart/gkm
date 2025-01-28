using UnityEngine;

public class WalkState : State
{
    // private Animator animator;
    private Transform playerTransform;
    private float moveSpeed = 5f;

    public WalkState(StateMachine stateMachine) : base(stateMachine)
    {
        // this.animator = animator;
        this.playerTransform = stateMachine.transform;
    }

    public override void Enter()
    {
        base.Enter();
        // animator.SetTrigger("Walk");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (direction.magnitude < 0.1f)
        {
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        playerTransform.Translate(direction * moveSpeed * Time.fixedDeltaTime, Space.World);
    }
}