using UnityEngine;

public class LootItem : MonoBehaviour
{
    public Item item;
    public int quantity; // Jumlah item dalam loot ini

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventoryManager = other.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(item, quantity);
                Destroy(gameObject); // Hancurkan objek loot setelah diambil
            }
        }
    }
}
