using UnityEngine;

/// <summary>
/// Contoh implementasi IInteractable untuk objek custom
/// Bisa digunakan untuk door, chest, button, switch, dll.
/// </summary>
public class ExampleInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactionText = "Press E to interact";
    [SerializeField] private bool canInteract = true;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject highlightEffect;
    [SerializeField] private Material highlightMaterial;
    
    private bool isHighlighted = false;
    private Renderer objectRenderer;
    private Material originalMaterial;
    
    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
    }
    
    #region IInteractable Implementation
    
    public bool CanInteract()
    {
        return canInteract;
    }
    
    public void Interact(PlayerStateMachine player)
    {
        if (!CanInteract()) return;
        
        Debug.Log($"Player interacted with {gameObject.name}!");
        
        // Contoh action: toggle active state
        canInteract = !canInteract;
        
        // Atau bisa melakukan action lain seperti:
        // - Membuka door
        // - Mengambil item dari chest
        // - Mengaktifkan switch
        // - Memulai dialogue
        // - dll.
    }
    
    public string GetInteractionText()
    {
        return interactionText;
    }
    
    public void OnLookAt()
    {
        if (!isHighlighted)
        {
            isHighlighted = true;
            
            // Aktifkan highlight effect
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(true);
            }
            
            // Atau ubah material
            if (objectRenderer != null && highlightMaterial != null)
            {
                objectRenderer.material = highlightMaterial;
            }
        }
    }
    
    public void OnLookAway()
    {
        if (isHighlighted)
        {
            isHighlighted = false;
            
            // Nonaktifkan highlight effect
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(false);
            }
            
            // Kembalikan material asli
            if (objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial;
            }
        }
    }
    
    #endregion
}
