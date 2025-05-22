using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public EquipmentManager equipmentManager; // Pastikan sudah direferensikan di Inspector

    // Method untuk menambahkan item ke inventory
    public void AddItem(Item newItem, int quantity)
    {
        // Cari apakah item sudah ada di inventory
        InventoryItem existingItem = items.Find(i => i.item.itemName == newItem.itemName);

        if (existingItem != null)
        {
            // Jika item sudah ada dan bisa ditumpuk, tambahkan jumlahnya
            if (existingItem.item.maxStack > existingItem.quantity)
            {
                int totalQuantity = existingItem.quantity + quantity;
                if (totalQuantity <= existingItem.item.maxStack)
                {
                    existingItem.quantity = totalQuantity;
                }
                else
                {
                    existingItem.quantity = existingItem.item.maxStack;
                    int remainder = totalQuantity - existingItem.item.maxStack;
                    AddItem(newItem, remainder); // Tambahkan sisa item ke slot baru
                }
            }
            else
            {
                Debug.Log("Stack penuh, tidak bisa menambahkan lebih banyak item ini.");
            }
        }
        else
        {
            Debug.Log("Item baru ditambahkan ke inventory.");
            // Jika item belum ada di inventory, tambahkan sebagai item baru
            InventoryItem inventoryItem = new InventoryItem(newItem, quantity);
            items.Add(inventoryItem);
        }

        Debug.Log($"Item {newItem.itemName} ditambahkan sebanyak {quantity}.");

        // Jika item bertipe Weapon, langsung equip secara otomatis
        if (newItem.itemType == ItemType.Weapon)
        {
            equipmentManager.EquipWeaponToRandomSlot(newItem);
        }

        // Quest system integration
        GKM.QuestSystem.QuestManager questManager = GKM.QuestSystem.QuestManager.Instance;
        if (questManager != null)
        {
            // Copy list agar aman dari modifikasi saat iterasi
            var questInstances = new List<GKM.QuestSystem.QuestInstance>(questManager.activeQuests);
            foreach (var questInstance in questInstances)
            {
                for (int i = 0; i < questInstance.objectives.Count; i++)
                {
                    var obj = questInstance.objectives[i];
                    if (obj.template.objectiveType == GKM.QuestSystem.ObjectiveType.CollectItem && obj.template.targetPrefab != null && !obj.IsCompleted)
                    {
                        Debug.Log($"[QuestCheck] Cek quest CollectItem: {obj.template.targetPrefab?.name} vs {newItem.itemName}, sebelum: {obj.currentAmount}, tambah: {quantity}, selesai: {obj.IsCompleted}");
                        obj.currentAmount = Mathf.Min(obj.currentAmount + quantity, obj.template.requiredAmount);
                        Debug.Log($"[QuestCheck] Setelah tambah: {obj.currentAmount}/{obj.template.requiredAmount}");
                        if (!obj.IsCompleted && obj.currentAmount >= obj.template.requiredAmount)
                        {
                            // Optionally: show notification objective complete
                        }
                        if (questInstance.IsCompleted)
                        {
                            questManager.CompleteQuest(questInstance);
                        }
                    }
                }
            }
        }
    }
}