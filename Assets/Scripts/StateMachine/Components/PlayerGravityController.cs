using UnityEngine;

/// <summary>
/// Komponen untuk mengelola sistem gravitasi pemain.
/// </summary>
public class PlayerGravityController : MonoBehaviour
{
    [SerializeField]
    private bool isGravityEnabled = true;
    private float originalGravity;
    private PlayerMovementConfig movementConfig;

    public bool IsGravityEnabled => isGravityEnabled;

    public void Initialize(PlayerMovementConfig config)
    {
        movementConfig = config;
        originalGravity = config.Gravity;
    }

    /// <summary>
    /// Mengaktifkan atau mematikan gravitasi pada player.
    /// </summary>
    /// <param name="enabled">True untuk mengaktifkan gravitasi, false untuk mematikan</param>
    public void SetGravityEnabled(bool enabled)
    {
        isGravityEnabled = enabled;
    }

    /// <summary>
    /// Toggle gravitasi on/off.
    /// </summary>
    public void ToggleGravity()
    {
        SetGravityEnabled(!isGravityEnabled);
    }

    /// <summary>
    /// Mendapatkan nilai gravitasi yang sedang aktif.
    /// </summary>
    /// <returns>Nilai gravitasi saat ini (0 jika dimatikan, nilai asli jika diaktifkan)</returns>
    public float GetCurrentGravity()
    {
        return isGravityEnabled ? originalGravity : 0f;
    }

    /// <summary>
    /// Mengembalikan gravitasi ke nilai asli dan mengaktifkannya.
    /// </summary>
    public void ResetGravity()
    {
        SetGravityEnabled(true);
    }
}
