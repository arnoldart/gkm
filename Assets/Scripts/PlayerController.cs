using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementConfig _movementConfig;
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

        // Inisialisasi Rigidbody
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

        // Inisialisasi
        _stateMachine = gameObject.AddComponent<StateMachine>();
        _stateMachine.ChangeState(new IdleState(_stateMachine));
    }

    private void Update()
    {
        if (_inputReader == null) return;

        Vector2 movement = _inputReader.MovementValue;

        if (movement.magnitude > 0.1f)
        {
            if (_inputReader.IsRunning) // Tambahkan event untuk Running
            {
                Debug.Log("Running");
                _stateMachine.ChangeState(new RunningState(_stateMachine, transform, _playerRigidbody, movement, _movementConfig));
            }
            else
            {
                Debug.Log("Walking");
                _stateMachine.ChangeState(new WalkState(_stateMachine, transform, _playerRigidbody, movement, _movementConfig));
            }
        }
        else
        {
            Debug.Log("Idle");
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
    }

}