using UnityEngine;

public class LootItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public Item item;
    public int quantity;

    [HideInInspector] public int damage;
    [HideInInspector] public int durability;
    [HideInInspector] public int maxStack;
    [HideInInspector] public int healthRestored;
    
    [Header("Interaction Settings")]
    [SerializeField] private string interactionText = "Press E to pickup";
    [SerializeField] private bool requireInteraction = true; // Jika false, auto pickup seperti sebelumnya
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject highlightEffect; // Outline atau highlight effect
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.1f;
      private Vector3 startPosition;
    private bool isHighlighted = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Bobbing animation
        if (bobHeight > 0)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnValidate()
    {
        if (item != null)
        {
            if (item.itemType == ItemType.Weapon)
            {
                item.damage = damage;
                item.durability = durability;
            }
            else if (item.itemType == ItemType.Consumable)
            {
                item.maxStack = maxStack;
                item.healthRestored = healthRestored;
            }
        }
    }    private void OnTriggerEnter(Collider other)
    {
        // Hanya auto pickup jika requireInteraction = false
        if (!requireInteraction && other.CompareTag("Player"))
        {
            InventoryManager inventoryManager = other.GetComponent<InventoryManager>();

            if (inventoryManager != null)
            {
                inventoryManager.AddItem(item, quantity);
                this.gameObject.SetActive(false);
            }
        }
    }

    #region IInteractable Implementation
    
    public bool CanInteract()
    {
        return item != null && requireInteraction;
    }
    
    public void Interact(PlayerStateMachine player)
    {
        if (!CanInteract()) return;
        
        InventoryManager inventoryManager = player.GetComponent<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(item, quantity);
            Debug.Log($"Picked up {quantity}x {item.itemName}");
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("InventoryManager not found on player!");
        }
    }
    
    public string GetInteractionText()
    {
        if (item != null)
        {
            return $"{interactionText} {item.itemName}";
        }
        return interactionText;
    }
    
    public void OnLookAt()
    {
        if (!isHighlighted)
        {
            isHighlighted = true;
            
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(true);
            }
            
            // Alternatif: Outline menggunakan material
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Bisa ditambahkan outline shader atau material highlight
                // renderer.material.SetFloat("_OutlineWidth", 0.1f);
            }
        }
    }
    
    public void OnLookAway()
    {
        if (isHighlighted)
        {
            isHighlighted = false;
            
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(false);
            }
            
            // Alternatif: Remove outline
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // renderer.material.SetFloat("_OutlineWidth", 0f);
            }
        }
    }
    
    #endregion
}
