using UnityEngine;

public class JumpState : State
{
    private readonly Transform _playerTransform;
    private readonly Rigidbody _playerRigidbody;
    private readonly Vector2 _moveInput;
    private readonly MovementConfig _config;
    private readonly Transform _cameraTransform;
    
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _isJumping;
    private static bool _hasJumped = false;

    public JumpState(StateMachine stateMachine, Transform playerTransform, Rigidbody playerRigidbody, Vector2 moveInput, MovementConfig config, Transform cameraTransform)
        : base(stateMachine)
    {
        _playerTransform = playerTransform;
        _playerRigidbody = playerRigidbody;
        _config = config;
        _cameraTransform = cameraTransform;
        _moveInput = moveInput;
    }

    public override void Enter()
    {
        base.Enter();
        
        if (_hasJumped) return;

        _coyoteTimeCounter = _config.coyoteTime;
        _jumpBufferCounter = _config.jumpBufferTime;
        _isJumping = true;
        _hasJumped = true; 

        _playerRigidbody.linearVelocity = new Vector3(
            _playerRigidbody.linearVelocity.x,
            _config.jumpForce,
            _playerRigidbody.linearVelocity.z
        );
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (!IsGrounded()) _coyoteTimeCounter -= Time.deltaTime;
        if (_jumpBufferCounter > 0) _jumpBufferCounter -= Time.deltaTime;

        // if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0)
        // {
        //     _stateMachine.ChangeState(new JumpState(_stateMachine, _playerTransform, _playerRigidbody, _moveInput, _config, _cameraTransform));
        // }

        IsGrounded();
        
        if (IsGrounded())
        {
            Debug.Log("Pemain menyentuh tanah. Reset hasJumped.");
            _hasJumped = false;
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
        
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 airControlDirection = (cameraRight * _moveInput.x + cameraForward * _moveInput.y).normalized;

        if (airControlDirection.magnitude > 0.1f)
        {
            Vector3 airMovement = airControlDirection * _config.airControlSpeed;
            _playerRigidbody.AddForce(airMovement, ForceMode.Acceleration);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _isJumping = false;
    }

    public bool IsGrounded()
    {
        Vector3 rayOrigin = _playerTransform.position;
        Vector3 rayDirection = Vector3.down;
        
        Debug.Log("TESTT APAKAH INI BISA");
    
        // Debugging visual: menampilkan garis raycast di Scene view
        Debug.DrawRay(rayOrigin, rayDirection * 10f, Color.red);

        bool hitGround = Physics.Raycast(rayOrigin, rayDirection, _config.groundCheckDistance, _config.groundLayer);

        if (hitGround)
        {
            Debug.Log("Raycast: Ground Terdeteksi!", _playerTransform.gameObject);
        }
        else
        {
            Debug.Log("Raycast: Tidak Menyentuh Ground", _playerTransform.gameObject);
        }

        return hitGround;
    }
}
