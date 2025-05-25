using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Demo UI untuk mengontrol gravitasi player.
/// Berguna untuk testing dan debugging.
/// </summary>
public class GravityDebugUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button toggleGravityButton;
    [SerializeField] private Button enableGravityButton;
    [SerializeField] private Button disableGravityButton;
    [SerializeField] private Button resetGravityButton;
    [SerializeField] private Text gravityStatusText;
    [SerializeField] private Text currentGravityValueText;
    
    [Header("Settings")]
    [SerializeField] private bool updateUI = true;
    [SerializeField] private float updateInterval = 0.1f;
    
    private PlayerStateMachine playerStateMachine;
    private float lastUpdateTime;
    
    private void Start()
    {
        // Cari PlayerStateMachine
        playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        if (playerStateMachine == null)
        {
            Debug.LogError("PlayerStateMachine tidak ditemukan!");
            return;
        }
        
        // Setup button events
        SetupButtons();
        
        // Update UI pertama kali
        UpdateUI();
    }
    
    private void Update()
    {
        if (updateUI && Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }
    
    private void SetupButtons()
    {
        if (toggleGravityButton != null)
        {
            toggleGravityButton.onClick.AddListener(ToggleGravity);
        }
        
        if (enableGravityButton != null)
        {
            enableGravityButton.onClick.AddListener(EnableGravity);
        }
        
        if (disableGravityButton != null)
        {
            disableGravityButton.onClick.AddListener(DisableGravity);
        }
        
        if (resetGravityButton != null)
        {
            resetGravityButton.onClick.AddListener(ResetGravity);
        }
    }
    
    private void UpdateUI()
    {
        if (playerStateMachine == null) return;
        
        // Update status text
        if (gravityStatusText != null)
        {
            gravityStatusText.text = $"Gravity: {(playerStateMachine.IsGravityEnabled ? "ON" : "OFF")}";
            gravityStatusText.color = playerStateMachine.IsGravityEnabled ? Color.green : Color.red;
        }
        
        // Update gravity value text
        if (currentGravityValueText != null)
        {
            float currentGravity = playerStateMachine.GetCurrentGravity();
            currentGravityValueText.text = $"Current Gravity: {currentGravity:F2}";
        }
    }
    
    public void ToggleGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.ToggleGravity();
        }
    }
    
    public void EnableGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.SetGravityEnabled(true);
        }
    }
    
    public void DisableGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.SetGravityEnabled(false);
        }
    }
    
    public void ResetGravity()
    {
        if (playerStateMachine != null)
        {
            playerStateMachine.ResetGravity();
        }
    }
    
    private void OnDestroy()
    {
        // Cleanup button events
        if (toggleGravityButton != null)
            toggleGravityButton.onClick.RemoveAllListeners();
        if (enableGravityButton != null)
            enableGravityButton.onClick.RemoveAllListeners();
        if (disableGravityButton != null)
            disableGravityButton.onClick.RemoveAllListeners();
        if (resetGravityButton != null)
            resetGravityButton.onClick.RemoveAllListeners();
    }
}
