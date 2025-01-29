using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementConfig _movementConfig;
    public Transform cameraTransform;
    private InputReader _inputReader;
    private StateMachine _stateMachine;
    private Rigidbody _playerRigidbody;

    private void Start()
    {
        _inputReader = gameObject.AddComponent<InputReader>();
        if (_inputReader == null)
        {
            Debug.LogError("InputReader belum diset di Inspector!");
            return;
        }

        _playerRigidbody = GetComponent<Rigidbody>();
        if (_playerRigidbody == null)
        {
            Debug.LogError("Rigidbody tidak ditemukan pada Player!");
            return;
        }
        
        if (_movementConfig == null)
        {
            Debug.LogError("MovementConfig belum diset di Inspector!");
            return;
        }
        
        
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform belum diset di Inspector!");
            return;
        }

        // Inisialisasi
        _stateMachine = gameObject.AddComponent<StateMachine>();
        _stateMachine.ChangeState(new IdleState(_stateMachine));

        _inputReader.JumpEvent += OnJump;
    }

    private void Update()
    {
        if (_inputReader == null) return;

        Vector2 movement = _inputReader.MovementValue;

        if (movement.magnitude > 0.1f)
        {
            if (_inputReader.IsRunning)
            {
                Debug.Log("Running");
                _stateMachine.ChangeState(new RunningState(_stateMachine, transform, _playerRigidbody, movement, _movementConfig, cameraTransform));
            }
            else
            {
                Debug.Log("Walking");
                _stateMachine.ChangeState(new WalkState(_stateMachine, transform, _playerRigidbody, movement, _movementConfig, cameraTransform));
            }
        }
        else
        {
            Debug.Log("Idle");
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
    }

    public void OnJump()
    {
        
        _stateMachine.ChangeState(new JumpState(_stateMachine, transform, _playerRigidbody, _inputReader.MovementValue, _movementConfig, cameraTransform));
    }
    
    private void OnDestroy()
    {
        if (_inputReader != null)
        {
            _inputReader.JumpEvent -= OnJump;
        }
    }

}