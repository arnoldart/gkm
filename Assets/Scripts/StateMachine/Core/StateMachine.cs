using UnityEngine;

/// <summary>
/// Pengontrol state machine dasar yang mengelola transisi state.
/// </summary>
public class StateMachine : MonoBehaviour
{
    /// <summary>
    /// State yang aktif saat ini.
    /// </summary>
    public State CurrentState { get; private set; }

    /// <summary>
    /// Mengubah state saat ini ke state baru.
    /// </summary>
    /// <param name="newState">State baru untuk transisi.</param>
    public void ChangeState(State newState)
    {
        if (newState == null)
        {
            Debug.LogError("Tidak dapat mengubah ke state null.");
            return;
        }

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    /// <summary>
    /// Mengeksekusi logika update untuk state saat ini.
    /// </summary>
    protected virtual void Update()
    {
        CurrentState?.UpdateLogic();
    }

    /// <summary>
    /// Mengeksekusi logika fixed update untuk state saat ini.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        CurrentState?.UpdatePhysics();
    }
}