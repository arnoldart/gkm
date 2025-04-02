using UnityEngine;

public class LootItem : MonoBehaviour
{
    public Item item;
    public int quantity;

    [HideInInspector] public int damage;
    [HideInInspector] public int durability;
    
    [HideInInspector] public int maxStack;
    [HideInInspector] public int healthRestored;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventoryManager = other.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(item, quantity);
                Destroy(gameObject);
            }
        }
    }
}
