using UnityEngine;

/// <summary>
/// Konfigurasi gerakan untuk karakter pemain.
/// </summary>
[System.Serializable]
public class PlayerMovementConfig
{
    [Header("Kecepatan Gerakan")]
    [SerializeField]
    private float walkSpeed = 2f;

    [SerializeField]
    private float slowRunSpeed = 4f;

    [SerializeField]
    private float runSpeed = 7f;

    [SerializeField]
    private float jumpForce = 12f;

    [SerializeField]
    private float airControl = 3f;

    [SerializeField]
    private float acceleration = 5f;

    [Header("Gravitasi")]
    [SerializeField]
    private float gravity = -25f;

    [Header("Mode Scene")]
    [SerializeField]
    private bool walkScene = false;

    [SerializeField]
    private bool runScene = true;

    [Header("Momentum")]
    [SerializeField]
    private float jumpMomentumMultiplier = 1.2f;

    // Properties
    public float WalkSpeed => walkSpeed;
    public float SlowRunSpeed => slowRunSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float Gravity => gravity;
    public float AirControl => airControl;
    public float Acceleration => acceleration;
    public bool WalkScene => walkScene;
    public bool RunScene => runScene;
    public float JumpMomentumMultiplier => jumpMomentumMultiplier;
}
