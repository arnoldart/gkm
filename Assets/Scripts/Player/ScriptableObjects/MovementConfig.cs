using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Scriptable Objects/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [Header("Movement Settings")]
    [Tooltip("Speed when walking.")]
    public float walkSpeed = 3f;

    [Tooltip("Speed when running.")]
    public float runSpeed = 8f;

    [Header("Rotation Settings")]
    [Tooltip("How fast the character rotates.")]
    public float rotationSpeed = 15f;

    [Header("Smoothing Settings")]
    [Tooltip("Smoothing time for movement transitions.")]
    public float movementSmoothTime = 0.1f;

    [Tooltip("Smoothing time for rotation transitions.")]
    public float rotationSmoothTime = 0.05f;
    
    [Header("Advanced Jump Settings")]
    public float jumpForce = 8f;
    public float airControlSpeed = 2f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
}
