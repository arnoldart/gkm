using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private StateMachine stateMachine;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        stateMachine = gameObject.AddComponent<StateMachine>();

        // Set initial state
        stateMachine.ChangeState(new IdleState(stateMachine));
    }

    private void Update()
    {
        // Example input to trigger attack
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     stateMachine.ChangeState(new AttackState(stateMachine, animator));
        // }
    }
}