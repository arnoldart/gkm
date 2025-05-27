using System.Collections;
using UnityEngine;

/// <summary>
/// State untuk transisi halus player dari posisi saat ini ke startPos lalu ke targetPos
/// </summary>
public class TeleportState : PlayerBaseState
{    private GameObject startPosObject;
    private GameObject targetPosObject;
    private float transitionDuration = 1.0f; // Durasi untuk setiap transisi (dalam detik)
    private float currentTimer = 0f;
    private bool isFirstPhase = true; // true = menuju startPos, false = menuju targetPos
    
    private Vector3 initialPosition;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Quaternion startRotation;
    private Quaternion targetRotation;
      public TeleportState(PlayerStateMachine stateMachine, GameObject startPosObject, GameObject targetPosObject, float duration = 1.0f) : base(stateMachine)
    {
        this.startPosObject = startPosObject;
        this.targetPosObject = targetPosObject;
        this.transitionDuration = duration;
    }    public override void Enter()
    {
        // Simpan posisi dan rotasi awal
        initialPosition = PlayerStateMachine.transform.position;
        initialRotation = PlayerStateMachine.transform.rotation;
        
        // Dapatkan posisi dan rotasi dari objek referensi
        startPosition = startPosObject.transform.position;
        startRotation = startPosObject.transform.rotation;
        targetPosition = targetPosObject.transform.position;
        targetRotation = targetPosObject.transform.rotation;
        
        // Mainkan animasi idle untuk transisi yang halus
        PlayAnimation("Idle", 0.2f);
        
        // Reset timer dan mulai dari fase pertama
        currentTimer = 0f;
        isFirstPhase = true;
        
        // Set flag untuk mencegah komponen lain mengoverride Controller
        PlayerStateMachine.PreventControllerOverride = true;
        
        // Matikan CharacterController untuk transisi halus
        PlayerStateMachine.Controller.enabled = false;
        
        Debug.Log("TeleportState: Starting smooth transition to startPos then targetPos");
    }

    public override void UpdateLogic()
    {
        currentTimer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(currentTimer / transitionDuration);

        if (isFirstPhase)
        {
            // Fase 1: Transisi dari posisi awal ke startPos menggunakan SetPosition yang aman
            Vector3 targetPos = Vector3.Lerp(initialPosition, startPosition, SmoothStep(normalizedTime));
            Quaternion targetRot = Quaternion.Slerp(initialRotation, startRotation, SmoothStep(normalizedTime));

            // Gunakan SetPosition yang aman untuk CharacterController
            SetPlayerPosition(targetPos);
            PlayerStateMachine.transform.rotation = targetRot;

            // Jika fase pertama selesai, lanjut ke fase kedua
            if (normalizedTime >= 1f)
            {
                isFirstPhase = false;
                currentTimer = 0f;

                // Update posisi untuk fase kedua
                initialPosition = startPosition;
                initialRotation = startRotation;

                Debug.Log("TeleportState: First phase complete, starting transition to targetPos");
            }
        }
        else
        {
            // Fase 2: Transisi dari startPos ke targetPos menggunakan SetPosition yang aman
            Vector3 targetPos = Vector3.Lerp(startPosition, targetPosition, SmoothStep(normalizedTime));
            Quaternion targetRot = Quaternion.Slerp(startRotation, targetRotation, SmoothStep(normalizedTime));

            // Gunakan SetPosition yang aman untuk CharacterController
            SetPlayerPosition(targetPos);
            PlayerStateMachine.transform.rotation = targetRot;

            // Jika fase kedua selesai, kembali ke idle state
            if (normalizedTime >= 1f)
            {                // Pastikan posisi akhir tepat
                SetPlayerPosition(targetPosition);
                PlayerStateMachine.transform.rotation = targetRotation;

                Debug.Log("TeleportState: Teleport sequence complete, returning to IdleState");
                PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            }
        }
    }    public override void Exit()
    {
        // Reset flag untuk mengizinkan komponen lain mengatur Controller kembali
        PlayerStateMachine.PreventControllerOverride = false;
        
        // Aktifkan kembali CharacterController
        PlayerStateMachine.Controller.enabled = true;
        
        // Reset vertical velocity
        PlayerStateMachine.VerticalVelocity = 0f;
        
        Debug.Log("TeleportState: Exiting teleport state and re-enabling CharacterController");
    }
      /// <summary>
    /// Metode aman untuk mengubah posisi player tanpa menyebabkan masalah CharacterController
    /// </summary>
    /// <param name="newPosition">Posisi baru untuk player</param>
    private void SetPlayerPosition(Vector3 newPosition)
    {
        // Karena Controller sudah dimatikan di Enter(), kita bisa langsung set posisi
        PlayerStateMachine.transform.position = newPosition;
    }
    
    /// <summary>
    /// Fungsi interpolasi SmoothStep untuk gerakan lebih halus
    /// </summary>
    /// <param name="x">Nilai antara 0 dan 1</param>
    /// <returns>Nilai yang dihaluskan menggunakan kurva smoothstep</returns>
    private float SmoothStep(float x)
    {
        return x * x * (3 - 2 * x);
    }
}
