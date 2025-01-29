using UnityEngine;

public abstract class MovementState : State
{
    protected readonly Transform _playerTransform;
    protected readonly CharacterController _characterController;
    protected readonly Vector2 _moveInput;
    protected readonly MovementConfig _config;
    protected readonly Transform _cameraTransform;

    private bool _isGrounded;
    private Vector3 _groundNormal;

    protected MovementState(StateMachine stateMachine, Transform playerTransform, CharacterController characterController, Vector2 moveInput, MovementConfig config, Transform cameraTransform)
        : base(stateMachine)
    {
        _playerTransform = playerTransform;
        _characterController = characterController;
        _moveInput = moveInput;
        _config = config;
        _cameraTransform = cameraTransform;
    }

    protected abstract float GetSpeed(); // Kecepatan akan diatur di subclass

    public override void UpdatePhysics()
    {
        CheckGround(); // Memanggil CheckGround untuk memperbarui status grounded

        if (_moveInput.sqrMagnitude < 0.01f)
        {
            StopMovement();
            return;
        }

        Vector3 targetDirection = CalculateMovementDirection();
        MovePlayer(targetDirection);
        RotatePlayer(targetDirection);
    }
    
    


    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = _playerTransform.position + Vector3.up * 0.1f; // Mulai raycast sedikit di atas karakter
        Vector3 rayDirection = Vector3.down; // Arah raycast ke bawah
        float rayLength = 1.5f; // Panjang raycast

        // Cek apakah raycast mengenai tanah
        _isGrounded = Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, _config.groundLayer);

        if (_isGrounded)
        {
            _groundNormal = hit.normal; // Simpan normal tanah
            Debug.DrawRay(hit.point, hit.normal * 1.5f, Color.green); // Debug Normal Tanah
        }
        else
        {
            _groundNormal = Vector3.up; // Jika tidak di tanah, set normal ke atas
        }

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, _isGrounded ? Color.green : Color.red); // Debug Raycast
    }



    protected Vector3 CalculateMovementDirection()
    {
        Vector3 cameraForward = _cameraTransform.forward;
        Vector3 cameraRight = _cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        Vector3 moveDirection = (cameraRight * _moveInput.x + cameraForward * _moveInput.y).normalized;

        // Jika di tanah, sejajarkan dengan normal tanah
        return Vector3.ProjectOnPlane(moveDirection, _groundNormal).normalized;
    }

    private void MovePlayer(Vector3 direction)
    {
        float speed = GetSpeed();
        Vector3 velocity = direction * speed;

        if (_isGrounded)
        {
            _characterController.Move(velocity * Time.deltaTime);
        }
        else
        {
            _characterController.Move(velocity * Time.deltaTime * 0.98f); // Sedikit tahanan udara
        }
    }

    private void StopMovement()
    {
        _characterController.Move(Vector3.zero);
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        _playerTransform.rotation = Quaternion.Slerp(
            _playerTransform.rotation,
            targetRotation,
            _config.rotationSpeed * Time.deltaTime
        );
    }
    
    
}
