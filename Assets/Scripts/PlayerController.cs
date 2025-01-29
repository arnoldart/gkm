using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementConfig _movementConfig;
    [SerializeField] private Transform cameraTransform;
    
    private InputReader _inputReader;
    private StateMachine _stateMachine;
    private CharacterController _characterController;

    private Vector2 _currentMovement = Vector2.zero;
    private bool _isRunning = false;

    private void Awake()
    {
        _inputReader = gameObject.AddComponent<InputReader>();
        _characterController = GetComponent<CharacterController>();

        ValidateComponents();

        _stateMachine = gameObject.AddComponent<StateMachine>();
    }

    private void Start()
    {
        _stateMachine.ChangeState(new IdleState(_stateMachine));

        // Subscribing ke event input
        _inputReader.MoveEvent += HandleMovementState;
        _inputReader.RunningEvent += HandleRunningState;
        _inputReader.JumpEvent += HandleJumpState;
    }

    private void OnDestroy()
    {
        _inputReader.MoveEvent -= HandleMovementState;
        _inputReader.RunningEvent -= HandleRunningState;
        _inputReader.JumpEvent -= HandleJumpState;
    }

    private void HandleMovementState(Vector2 movement)
    {
        _currentMovement = movement;

        if (movement.sqrMagnitude > 0.01f)
        {
            State newState = _isRunning 
                ? new RunningState(_stateMachine, transform, _characterController, movement, _movementConfig, cameraTransform) 
                : new WalkState(_stateMachine, transform, _characterController, movement, _movementConfig, cameraTransform);
            
            _stateMachine.ChangeState(newState);
        }
        else
        {
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
    }

    private void HandleRunningState(bool isRunning)
    {
        _isRunning = isRunning;
        HandleMovementState(_currentMovement); // Update state saat status running berubah
    }

    private void HandleJumpState()
    {
        // _stateMachine.ChangeState(new JumpState(_stateMachine, transform, _characterController, _currentMovement, _movementConfig, cameraTransform));
    }

    private void ValidateComponents()
    {
        if (_inputReader == null) Debug.LogError("InputReader belum diset di Inspector!");
        if (_characterController == null) Debug.LogError("CharacterController tidak ditemukan pada Player!");
        if (_movementConfig == null) Debug.LogError("MovementConfig belum diset di Inspector!");
        if (cameraTransform == null) Debug.LogError("Camera Transform belum diset di Inspector!");
    }
}
