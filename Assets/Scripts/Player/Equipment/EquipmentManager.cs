using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipment Slots")]
    public Item equippedWeapon; // Slot untuk weapon

    // Reference ke Input Action (bisa di-setup lewat kode atau melalui asset)
    private InputAction weaponShortcutAction;

    private void Awake()
    {
        // Membuat InputAction Map secara dinamis untuk keperluan demo.
        // Di project nyata, sebaiknya gunakan PlayerInput atau Input Action Asset yang sudah dibuat di editor.
        var map = new InputActionMap("Equipment");
        weaponShortcutAction = map.AddAction("WeaponShortcut", binding: "<Keyboard>/1");
        weaponShortcutAction.performed += ctx => OnWeaponShortcut();
        map.Enable();
    }

    // Fungsi untuk equip weapon
    public void EquipWeapon(Item weapon)
    {
        equippedWeapon = weapon;
        Debug.Log("Weapon ter-equipment: " + weapon.itemName);
    }

    // Fungsi untuk unequip weapon (jika dibutuhkan)
    public void UnequipWeapon()
    {
        if (equippedWeapon != null)
        {
            Debug.Log("Weapon dilepas: " + equippedWeapon.itemName);
            equippedWeapon = null;
        }
    }

    // Fungsi yang dipanggil saat shortcut ditekan
    private void OnWeaponShortcut()
    {
        if (equippedWeapon != null)
        {
            UseWeapon(equippedWeapon);
        }
        else
        {
            Debug.Log("Tidak ada weapon yang ter-equipment!");
        }
    }
    
    private void OnEnable()
    {
        // Pastikan action sudah di-enable
        weaponShortcutAction.Enable();
        weaponShortcutAction.performed += OnWeaponShortcut;
    }

    private void OnDisable()
    {
        weaponShortcutAction.performed -= OnWeaponShortcut;
        weaponShortcutAction.Disable();
    }

    private void OnWeaponShortcut(InputAction.CallbackContext context)
    {
        // Logika untuk penggunaan weapon
        Debug.Log("Tombol shortcut weapon ditekan!");
    }


    // Contoh implementasi penggunaan weapon
    private void UseWeapon(Item weapon)
    {
        Debug.Log("Menggunakan weapon: " + weapon.itemName);
        // Tambahkan logika penggunaan weapon (misal: menyerang, efek animasi, dsb.)
    }
    
    
}
