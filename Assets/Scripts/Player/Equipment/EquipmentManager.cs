using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipment Slots")]
    public Item equippedWeapon1; // Slot senjata 1
    public Item equippedWeapon2; // Slot senjata 2
    public Item equippedWeapon3; // Slot senjata 3
    public Item equippedWeapon4; // Slot senjata 4

    [Header("Active Weapon")]
    [SerializeField] private int activeWeaponSlot = 0; // 0 = tidak ada, 1-4 = slot aktif
    [SerializeField] private Item activeWeapon; // Referensi ke senjata yang sedang digunakan

    // Reference ke Input Action untuk setiap slot senjata
    private InputActionMap weaponActionMap;
    private InputAction weapon1ShortcutAction;
    private InputAction weapon2ShortcutAction;
    private InputAction weapon3ShortcutAction;
    private InputAction weapon4ShortcutAction;

    // Event untuk notifikasi UI saat senjata aktif berubah
    public delegate void WeaponChangedDelegate(Item weapon, int slot);
    public event WeaponChangedDelegate OnWeaponChanged;

    private void Awake()
    {
        // Membuat InputAction Map secara dinamis
        weaponActionMap = new InputActionMap("Weapons");
        
        // Setup semua shortcut untuk senjata
        weapon1ShortcutAction = weaponActionMap.AddAction("Weapon1Shortcut", binding: "<Keyboard>/1");
        weapon2ShortcutAction = weaponActionMap.AddAction("Weapon2Shortcut", binding: "<Keyboard>/2");
        weapon3ShortcutAction = weaponActionMap.AddAction("Weapon3Shortcut", binding: "<Keyboard>/3");
        weapon4ShortcutAction = weaponActionMap.AddAction("Weapon4Shortcut", binding: "<Keyboard>/4");
        
        // Enable action map
        weaponActionMap.Enable();
    }

    private void OnEnable()
    {
        // Subscribe ke semua event
        weapon1ShortcutAction.performed += OnWeapon1Shortcut;
        weapon2ShortcutAction.performed += OnWeapon2Shortcut;
        weapon3ShortcutAction.performed += OnWeapon3Shortcut;
        weapon4ShortcutAction.performed += OnWeapon4Shortcut;
    }

    private void OnDisable()
    {
        // Unsubscribe dari semua event
        weapon1ShortcutAction.performed -= OnWeapon1Shortcut;
        weapon2ShortcutAction.performed -= OnWeapon2Shortcut;
        weapon3ShortcutAction.performed -= OnWeapon3Shortcut;
        weapon4ShortcutAction.performed -= OnWeapon4Shortcut;
        
        weaponActionMap.Disable();
    }

    // SLOT 1 METHODS
    public void EquipWeapon1(Item weapon)
    {
        equippedWeapon1 = weapon;
        Debug.Log("Senjata 1 ter-equipment: " + weapon.itemName);
        
        // Jika belum ada senjata aktif, jadikan ini senjata aktif
        if (activeWeaponSlot == 0)
        {
            SetActiveWeapon(1);
        }
    }

    public void UnequipWeapon1()
    {
        if (equippedWeapon1 != null)
        {
            Debug.Log("Senjata 1 dilepas: " + equippedWeapon1.itemName);
            
            // Jika ini senjata aktif, reset senjata aktif
            if (activeWeaponSlot == 1)
            {
                ClearActiveWeapon();
            }
            
            equippedWeapon1 = null;
        }
    }

    private void OnWeapon1Shortcut(InputAction.CallbackContext context)
    {
        if (equippedWeapon1 != null)
        {
            SetActiveWeapon(1); // Set sebagai senjata aktif saat shortcut dipicu
            UseWeapon(equippedWeapon1, 1);
        }
        else
        {
            Debug.Log("Tidak ada senjata yang ter-equipment di slot 1!");
        }
    }

    // SLOT 2 METHODS
    public void EquipWeapon2(Item weapon)
    {
        equippedWeapon2 = weapon;
        Debug.Log("Senjata 2 ter-equipment: " + weapon.itemName);
        
        // Jika belum ada senjata aktif, jadikan ini senjata aktif
        if (activeWeaponSlot == 0)
        {
            SetActiveWeapon(2);
        }
    }

    public void UnequipWeapon2()
    {
        if (equippedWeapon2 != null)
        {
            Debug.Log("Senjata 2 dilepas: " + equippedWeapon2.itemName);
            
            // Jika ini senjata aktif, reset senjata aktif
            if (activeWeaponSlot == 2)
            {
                ClearActiveWeapon();
            }
            
            equippedWeapon2 = null;
        }
    }

    private void OnWeapon2Shortcut(InputAction.CallbackContext context)
    {
        if (equippedWeapon2 != null)
        {
            SetActiveWeapon(2); // Set sebagai senjata aktif saat shortcut dipicu
            UseWeapon(equippedWeapon2, 2);
        }
        else
        {
            Debug.Log("Tidak ada senjata yang ter-equipment di slot 2!");
        }
    }

    // SLOT 3 METHODS
    public void EquipWeapon3(Item weapon)
    {
        equippedWeapon3 = weapon;
        Debug.Log("Senjata 3 ter-equipment: " + weapon.itemName);
        
        // Jika belum ada senjata aktif, jadikan ini senjata aktif
        if (activeWeaponSlot == 0)
        {
            SetActiveWeapon(3);
        }
    }

    public void UnequipWeapon3()
    {
        if (equippedWeapon3 != null)
        {
            Debug.Log("Senjata 3 dilepas: " + equippedWeapon3.itemName);
            
            // Jika ini senjata aktif, reset senjata aktif
            if (activeWeaponSlot == 3)
            {
                ClearActiveWeapon();
            }
            
            equippedWeapon3 = null;
        }
    }

    private void OnWeapon3Shortcut(InputAction.CallbackContext context)
    {
        if (equippedWeapon3 != null)
        {
            SetActiveWeapon(3); // Set sebagai senjata aktif saat shortcut dipicu
            UseWeapon(equippedWeapon3, 3);
        }
        else
        {
            Debug.Log("Tidak ada senjata yang ter-equipment di slot 3!");
        }
    }

    // SLOT 4 METHODS
    public void EquipWeapon4(Item weapon)
    {
        equippedWeapon4 = weapon;
        Debug.Log("Senjata 4 ter-equipment: " + weapon.itemName);
        
        // Jika belum ada senjata aktif, jadikan ini senjata aktif
        if (activeWeaponSlot == 0)
        {
            SetActiveWeapon(4);
        }
    }

    public void UnequipWeapon4()
    {
        if (equippedWeapon4 != null)
        {
            Debug.Log("Senjata 4 dilepas: " + equippedWeapon4.itemName);
            
            // Jika ini senjata aktif, reset senjata aktif
            if (activeWeaponSlot == 4)
            {
                ClearActiveWeapon();
            }
            
            equippedWeapon4 = null;
        }
    }

    private void OnWeapon4Shortcut(InputAction.CallbackContext context)
    {
        if (equippedWeapon4 != null)
        {
            SetActiveWeapon(4); // Set sebagai senjata aktif saat shortcut dipicu
            UseWeapon(equippedWeapon4, 4);
        }
        else
        {
            Debug.Log("Tidak ada senjata yang ter-equipment di slot 4!");
        }
    }

    // Method untuk mengatur senjata aktif
    public void SetActiveWeapon(int slotNumber)
    {
        // Pastikan slot valid (1-4)
        if (slotNumber < 1 || slotNumber > 4)
        {
            Debug.LogError("Slot senjata tidak valid: " + slotNumber);
            return;
        }
        
        // Simpan slot aktif saat ini
        activeWeaponSlot = slotNumber;
        
        // Update referensi ke senjata aktif
        switch (slotNumber)
        {
            case 1:
                activeWeapon = equippedWeapon1;
                break;
            case 2:
                activeWeapon = equippedWeapon2;
                break;
            case 3:
                activeWeapon = equippedWeapon3;
                break;
            case 4:
                activeWeapon = equippedWeapon4;
                break;
        }
        
        // Log perubahan senjata
        Debug.Log($"Senjata aktif sekarang: Slot {activeWeaponSlot} - {(activeWeapon != null ? activeWeapon.itemName : "Kosong")}");
        
        // Notifikasi UI dan sistem lain tentang perubahan senjata
        OnWeaponChanged?.Invoke(activeWeapon, activeWeaponSlot);
    }
    
    // Method untuk menghapus senjata aktif
    public void ClearActiveWeapon()
    {
        activeWeaponSlot = 0;
        activeWeapon = null;
        
        // Notifikasi tentang tidak ada senjata aktif
        OnWeaponChanged?.Invoke(null, 0);
        
        Debug.Log("Tidak ada senjata aktif");
    }
    
    // Method untuk mendapatkan senjata aktif saat ini
    public Item GetActiveWeapon()
    {
        return activeWeapon;
    }
    
    // Method untuk mendapatkan slot senjata aktif saat ini
    public int GetActiveWeaponSlot()
    {
        return activeWeaponSlot;
    }

    // Fungsi umum untuk menggunakan senjata
    private void UseWeapon(Item weapon, int slotNumber)
    {
        Debug.Log($"Menggunakan senjata dari slot {slotNumber}: {weapon.itemName}");
        
        // Logika penggunaan senjata disini
        // Tambahkan logika seperti menyerang, efek animasi, dsb.
        if (weapon.damage > 0)
        {
            Debug.Log($"Damage senjata: {weapon.damage}");
        }
        
        if (weapon.durability > 0)
        {
            Debug.Log($"Durability senjata: {weapon.durability}");
            // Implementasi logika pengurangan durability bila perlu
        }
    }

    // Fungsi untuk memberikan senjata ke slot yang tersedia
    public void EquipWeaponToFirstAvailableSlot(Item weapon)
    {
        if (equippedWeapon1 == null)
        {
            EquipWeapon1(weapon);
        }
        else if (equippedWeapon2 == null)
        {
            EquipWeapon2(weapon);
        }
        else if (equippedWeapon3 == null)
        {
            EquipWeapon3(weapon);
        }
        else if (equippedWeapon4 == null)
        {
            EquipWeapon4(weapon);
        }
        else
        {
            Debug.Log("Semua slot senjata sudah terisi!");
        }
    }

    // Fungsi untuk memberikan senjata ke slot secara acak
    public void EquipWeaponToRandomSlot(Item weapon)
    {
        // Dapatkan daftar slot kosong
        List<int> availableSlots = new List<int>();
        
        if (equippedWeapon1 == null) availableSlots.Add(1);
        if (equippedWeapon2 == null) availableSlots.Add(2);
        if (equippedWeapon3 == null) availableSlots.Add(3);
        if (equippedWeapon4 == null) availableSlots.Add(4);
        
        // Jika semua slot sudah terisi, pilih slot acak untuk diganti
        if (availableSlots.Count == 0)
        {
            int randomSlot = Random.Range(1, 5); // 1-4
            Debug.Log($"Semua slot sudah terisi! Mengganti senjata di slot {randomSlot}.");
            
            switch (randomSlot)
            {
                case 1:
                    EquipWeapon1(weapon);
                    break;
                case 2:
                    EquipWeapon2(weapon);
                    break;
                case 3:
                    EquipWeapon3(weapon);
                    break;
                case 4:
                    EquipWeapon4(weapon);
                    break;
            }
        }
        // Jika ada slot kosong, pilih salah satu secara acak
        else
        {
            int randomIndex = Random.Range(0, availableSlots.Count);
            int chosenSlot = availableSlots[randomIndex];
            
            Debug.Log($"Memasukkan senjata ke slot acak: {chosenSlot}");
            
            switch (chosenSlot)
            {
                case 1:
                    EquipWeapon1(weapon);
                    break;
                case 2:
                    EquipWeapon2(weapon);
                    break;
                case 3:
                    EquipWeapon3(weapon);
                    break;
                case 4:
                    EquipWeapon4(weapon);
                    break;
            }
        }
    }
}
