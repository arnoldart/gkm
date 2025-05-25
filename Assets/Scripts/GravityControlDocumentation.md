# Gravity Control System - Documentation

## Overview
Sistem kontrol gravitasi untuk PlayerStateMachine yang memungkinkan Anda mengaktifkan/mematikan gravitasi secara dinamis selama gameplay.

## Features
- Toggle gravitasi on/off
- Reset gravitasi ke nilai asli
- Kontrol keyboard untuk testing (tombol G)
- Method publik untuk kontrol eksternal
- Kompatibel dengan semua player states

## Files Modified/Created
1. `PlayerStateMachine.cs` - Menambahkan kontrol gravitasi utama
2. `PlayerBaseState.cs` - Memodifikasi ApplyGravity() untuk menggunakan kontrol baru
3. `GravityController.cs` - Script utility untuk kontrol eksternal
4. `GravityDebugUI.cs` - UI debug untuk testing

## Quick Usage

### 1. In-Game Controls
- **G Key**: Toggle gravitasi on/off (untuk testing)
- **Escape Key**: Toggle cursor lock (existing)

### 2. Script Access
```csharp
// Mendapatkan reference PlayerStateMachine
PlayerStateMachine playerSM = FindObjectOfType<PlayerStateMachine>();

// Mematikan gravitasi
playerSM.SetGravityEnabled(false);

// Mengaktifkan gravitasi
playerSM.SetGravityEnabled(true);

// Toggle gravitasi
playerSM.ToggleGravity();

// Reset ke nilai asli
playerSM.ResetGravity();

// Cek status gravitasi
bool isEnabled = playerSM.IsGravityEnabled;

// Mendapatkan nilai gravitasi saat ini
float currentGravity = playerSM.GetCurrentGravity();
```

### 3. Trigger Zone (Menggunakan GravityController)
```csharp
// Attach GravityController ke GameObject dengan Collider (IsTrigger = true)
public class MyTrigger : MonoBehaviour
{
    private GravityController gravityController;
    
    void Start()
    {
        gravityController = GetComponent<GravityController>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gravityController.DisableGravity();
        }
    }
}
```

### 4. UI Integration (Menggunakan GravityDebugUI)
```csharp
// Attach ke Canvas dan assign button references di inspector
// Buttons akan otomatis berfungsi untuk kontrol gravitasi
```

## Public Methods di PlayerStateMachine

### SetGravityEnabled(bool enabled)
Mengaktifkan atau mematikan gravitasi.
- `enabled`: true untuk aktif, false untuk mati
- Ketika dimatikan, vertical velocity di-reset ke 0

### ToggleGravity()
Membalik status gravitasi (on menjadi off, off menjadi on).

### GetCurrentGravity()
Mengembalikan nilai gravitasi saat ini.
- Returnsnts 0 jika gravitasi dimatikan
- Returns nilai asli jika gravitasi diaktifkan

### ResetGravity()
Mengembalikan gravitasi ke nilai asli dan mengaktifkannya.

## Public Properties

### IsGravityEnabled
Boolean yang menunjukkan status gravitasi saat ini.

## Implementation Details

### PlayerStateMachine Changes
- Added `IsGravityEnabled` property
- Added `originalGravity` field untuk menyimpan nilai asli
- Modified `Awake()` untuk inisialisasi
- Modified `Update()` untuk kontrol keyboard
- Added gravitasi control methods

### PlayerBaseState Changes
- Modified `ApplyGravity()` untuk cek `IsGravityEnabled`
- Menggunakan `GetCurrentGravity()` instead of direct access

## Best Practices

1. **Performance**: Gravitasi control sangat ringan, safe untuk digunakan setiap frame
2. **State Consistency**: Gravitasi otomatis di-handle di semua player states
3. **Visual Feedback**: Gunakan Debug.Log atau UI untuk memberikan feedback
4. **Reset**: Selalu reset gravitasi saat player respawn atau scene change

## Example Use Cases

1. **Zero Gravity Zones**: Area luar angkasa atau underwater
2. **Puzzle Mechanics**: Platform yang membutuhkan floating
3. **Special Abilities**: Power-up atau magic spell
4. **Cutscenes**: Kontrol penuh atas player movement
5. **Debug/Testing**: Easy testing tanpa perlu modify script

## Troubleshooting

### Player Masih Jatuh Setelah Gravitasi Dimatikan
- Pastikan `VerticalVelocity` di-reset saat gravitasi dimatikan
- Cek apakah ada force lain yang mempengaruhi player

### Gravitasi Tidak Berfungsi
- Pastikan `originalGravity` ter-set dengan benar di `Awake()`
- Cek apakah `IsGravityEnabled` property memiliki nilai yang benar

### Performance Issues
- Sistem ini sangat lightweight, jika ada masalah performance kemungkinan dari bagian lain

## Advanced Usage

### Custom Gravity Values
```csharp
// Simpan nilai asli
float originalGrav = playerSM.GetCurrentGravity();

// Set custom gravity (modify field langsung)
// Note: Ini akan ter-override saat ResetGravity() dipanggil
```

### Conditional Gravity
```csharp
// Hanya matikan gravitasi saat kondisi tertentu
if (playerIsInWater && !playerIsSwimming)
{
    playerSM.SetGravityEnabled(false);
}
```
