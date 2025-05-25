using UnityEngine;

/// <summary>
/// Controller untuk mengontrol gravitasi player secara eksternal.
/// Berguna untuk area khusus, trigger, atau gameplay mechanics.
/// </summary>
public class GravityController : MonoBehaviour
{
    [Header("Gravity Zone Settings")]
    [SerializeField] private bool disableGravityOnEnter = true;
    [SerializeField] private bool enableGravityOnExit = true;
    [SerializeField] private bool isGravityZone = false;
    
    [Header("Manual Control")]
    [SerializeField] private KeyCode toggleKey = KeyCode.H;
    [SerializeField] private bool enableManualControl = true;
    
    private PlayerStateMachine playerStateMachine;
    
    private void Start()
    {
        // Cari PlayerStateMachine di scene
        playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        if (playerStateMachine == null)
        {
            Debug.LogWarning("PlayerStateMachine tidak ditemukan di scene!");
        }
    }
    
    private void Update()
    {
        // Manual control untuk testing
        if (enableManualControl && Input.GetKeyDown(toggleKey))
        {
            ToggleGravity();
        }
    }
    
    /// <summary>
    /// Toggle gravitasi on/off.
    /// </summary>
    public void ToggleGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.ToggleGravity();
            Debug.Log($"Gravity {(playerStateMachine.IsGravityEnabled ? "Enabled" : "Disabled")} by GravityController");
        }
    }
    
    /// <summary>
    /// Mengaktifkan gravitasi.
    /// </summary>
    public void EnableGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.SetGravityEnabled(true);
            Debug.Log("Gravity Enabled by GravityController");
        }
    }
    
    /// <summary>
    /// Mematikan gravitasi.
    /// </summary>
    public void DisableGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.SetGravityEnabled(false);
            Debug.Log("Gravity Disabled by GravityController");
        }
    }
    
    /// <summary>
    /// Reset gravitasi ke nilai asli.
    /// </summary>
    public void ResetGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.ResetGravity();
            Debug.Log("Gravity Reset by GravityController");
        }
    }
    
    // Trigger events untuk area/zone based gravity control
    private void OnTriggerEnter(Collider other)
    {
        if (!isGravityZone) return;
        
        if (other.CompareTag("Player"))
        {
            if (disableGravityOnEnter)
            {
                DisableGravity();
                Debug.Log("Player entered gravity zone - Gravity disabled");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!isGravityZone) return;
        
        if (other.CompareTag("Player"))
        {
            if (enableGravityOnExit)
            {
                EnableGravity();
                Debug.Log("Player exited gravity zone - Gravity enabled");
            }
        }
    }
    
    // Method untuk dipanggil dari script lain atau UI
    public void SetGravityState(bool enabled)
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.SetGravityEnabled(enabled);
            Debug.Log($"Gravity {(enabled ? "Enabled" : "Disabled")} by external call");
        }
    }
}
