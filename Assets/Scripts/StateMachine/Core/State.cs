using UnityEngine;

/// <summary>
/// Kelas abstrak dasar untuk semua state dalam pola state machine.
/// </summary>
public abstract class State
{
    /// <summary>
    /// Referensi ke state machine induk.
    /// </summary>
    protected StateMachine StateMachine { get; }

    /// <summary>
    /// Konstruktor yang menerima state machine pengontrol.
    /// </summary>
    /// <param name="stateMachine">State machine yang memiliki state ini</param>
    protected State(StateMachine stateMachine)
    {
        StateMachine = stateMachine ?? throw new System.ArgumentNullException(nameof(stateMachine));
    }

    /// <summary>
    /// Dipanggil ketika state dimasuki.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Dipanggil ketika state keluar.
    /// </summary>
    public virtual void Exit() { }

    /// <summary>
    /// Dipanggil selama fase Update untuk menangani logika non-fisika.
    /// </summary>
    public virtual void UpdateLogic() { }

    /// <summary>
    /// Dipanggil selama fase FixedUpdate untuk menangani logika terkait fisika.
    /// </summary>
    public virtual void UpdatePhysics() { }
}