using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a health bar for any entity with a HealthSystem
/// </summary>
public class HealthUI : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private float hideDelay = 3f;
    [SerializeField] private bool followTarget = false;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    
    private float lastDamageTime;
    private CanvasGroup canvasGroup;
    private Camera mainCamera;
    
    private void Awake()
    {
        if (healthSystem == null)
        {
            healthSystem = GetComponentInParent<HealthSystem>();
        }
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && hideWhenFull)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        mainCamera = Camera.main;
    }
    
    private void Start()
    {
        if (healthSystem == null)
        {
            Debug.LogError("No HealthSystem found for HealthUI", this);
            enabled = false;
            return;
        }
        
        // Subscribe to health events
        healthSystem.OnHealthChanged += OnHealthChanged;
        
        // Initial state
        UpdateHealthBar();
        
        if (hideWhenFull && canvasGroup != null && healthSystem.CurrentHealth >= healthSystem.MaxHealth)
        {
            canvasGroup.alpha = 0;
        }
    }
    
    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= OnHealthChanged;
        }
    }
    
    private void Update()
    {
        if (followTarget && mainCamera != null)
        {
            // Make the UI face the camera
            transform.position = healthSystem.transform.position + offset;
            transform.forward = mainCamera.transform.forward;
        }
        
        // Hide the health bar after a delay if health is full
        if (hideWhenFull && canvasGroup != null && 
            healthSystem.CurrentHealth >= healthSystem.MaxHealth &&
            Time.time > lastDamageTime + hideDelay)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * 2);
        }
    }
    
    private void OnHealthChanged(int change, int currentHealth)
    {
        lastDamageTime = Time.time;
        
        if (hideWhenFull && canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
        
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar()
    {
        if (healthFillImage == null) return;
        
        float healthPercent = (float)healthSystem.CurrentHealth / healthSystem.MaxHealth;
        healthFillImage.fillAmount = healthPercent;
        
        // Optional: Change color based on health percentage
        if (healthPercent <= 0.2f)
        {
            healthFillImage.color = Color.red;
        }
        else if (healthPercent <= 0.5f)
        {
            healthFillImage.color = Color.yellow;
        }
        else
        {
            healthFillImage.color = Color.green;
        }
    }
}