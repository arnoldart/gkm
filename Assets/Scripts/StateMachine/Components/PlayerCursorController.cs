using UnityEngine;

/// <summary>
/// Komponen untuk mengelola kontrol kursor pemain.
/// </summary>
public class PlayerCursorController : MonoBehaviour
{
    private bool isCursorLocked = true;

    public bool IsCursorLocked => isCursorLocked;

    private void Update()
    {
        HandleCursorToggle();
    }

    /// <summary>
    /// Menangani toggle kursor dengan tombol Escape.
    /// </summary>
    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            SetCursorLock(isCursorLocked);
        }
    }

    /// <summary>
    /// Mengatur status lock kursor.
    /// </summary>
    /// <param name="locked">True untuk mengunci kursor, false untuk melepas</param>
    public void SetCursorLock(bool locked)
    {
        isCursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
