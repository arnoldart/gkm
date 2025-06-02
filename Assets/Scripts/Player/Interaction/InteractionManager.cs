using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mengelola objek-objek yang bisa diinteraksi oleh player
/// </summary>
public class InteractionManager : MonoBehaviour
{    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayers = -1;
    [SerializeField] private Transform raycastOrigin; // Optional: custom raycast origin point
    [SerializeField] private InteractionMethod detectionMethod = InteractionMethod.Raycast;
    [SerializeField] private float sphereDetectionRadius = 1.5f; // Untuk sphere detection
    
    public enum InteractionMethod
    {
        Raycast,        // Raycast ke arah depan player
        Sphere,         // Sphere detection di sekitar player
        Both            // Kombinasi keduanya
    }
    
    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt; // UI prompt "Press E to interact"
    
    private PlayerStateMachine playerStateMachine;
    private Transform playerTransform;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentInteractable;
    
    private void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerTransform = transform;
        
        // Jika tidak ada raycast origin yang diset, gunakan player transform
        if (raycastOrigin == null)
        {
            raycastOrigin = playerTransform;
        }
    }
    
    private void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }    /// <summary>
    /// Mencari objek yang bisa diinteraksi di sekitar player
    /// </summary>
    private void CheckForInteractables()
    {
        IInteractable detectedInteractable = null;
        
        switch (detectionMethod)
        {
            case InteractionMethod.Raycast:
                detectedInteractable = CheckRaycastInteraction();
                break;
            case InteractionMethod.Sphere:
                detectedInteractable = CheckSphereInteraction();
                break;
            case InteractionMethod.Both:
                // Prioritas: Raycast dulu, jika tidak ada baru sphere
                detectedInteractable = CheckRaycastInteraction();
                if (detectedInteractable == null)
                {
                    detectedInteractable = CheckSphereInteraction();
                }
                break;
        }
        
        // Update current interactable
        if (detectedInteractable != null && detectedInteractable.CanInteract())
        {
            if (currentInteractable != detectedInteractable)
            {
                // Highlight objek baru
                if (currentInteractable != null)
                {
                    currentInteractable.OnLookAway();
                }
                
                currentInteractable = detectedInteractable;
                currentInteractable.OnLookAt();
                ShowInteractionPrompt(true, detectedInteractable.GetInteractionText());
            }
        }
        else
        {
            ClearCurrentInteractable();
        }
    }
    
    /// <summary>
    /// Raycast detection untuk interaction
    /// </summary>
    private IInteractable CheckRaycastInteraction()
    {
        Vector3 rayOrigin = raycastOrigin.position + Vector3.up * 0.5f;
        Vector3 rayDirection = raycastOrigin.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, interactionRange, interactionLayers))
        {
            // Debug raycast - untuk debugging di Scene view
            Debug.DrawRay(rayOrigin, rayDirection * interactionRange, Color.yellow);
            return hit.collider.GetComponent<IInteractable>();
        }
        
        Debug.DrawRay(rayOrigin, rayDirection * interactionRange, Color.red);
        return null;
    }
    
    /// <summary>
    /// Sphere detection untuk interaction
    /// </summary>
    private IInteractable CheckSphereInteraction()
    {
        Collider[] colliders = Physics.OverlapSphere(raycastOrigin.position, sphereDetectionRadius, interactionLayers);
        
        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(raycastOrigin.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        
        return closestInteractable;
    }
    
    /// <summary>
    /// Menangani input interaksi
    /// </summary>
    private void HandleInteractionInput()
    {
        if (playerStateMachine.InputHandler.IsPressed(InputType.Interact))
        {
            if (currentInteractable != null && currentInteractable.CanInteract())
            {
                currentInteractable.Interact(playerStateMachine);
                playerStateMachine.InputHandler.ResetPressed(InputType.Interact);
                
                // Hapus highlight setelah interaksi
                ClearCurrentInteractable();
            }
        }
    }
    
    /// <summary>
    /// Menghapus interactable saat ini dan menyembunyikan prompt
    /// </summary>
    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnLookAway();
            currentInteractable = null;
            ShowInteractionPrompt(false);
        }
    }
    
    /// <summary>
    /// Menampilkan atau menyembunyikan interaction prompt
    /// </summary>
    private void ShowInteractionPrompt(bool show, string text = "")
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
            
            if (show && !string.IsNullOrEmpty(text))
            {
                // Update text jika ada Text component
                var textComponent = interactionPrompt.GetComponentInChildren<UnityEngine.UI.Text>();
                if (textComponent != null)
                {
                    textComponent.text = text;
                }
                
                // Atau TMPro text
                var tmpComponent = interactionPrompt.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmpComponent != null)
                {
                    tmpComponent.text = text;
                }
            }
        }
    }
    
    /// <summary>
    /// Menambahkan interactable ke dalam range
    /// </summary>
    public void AddNearbyInteractable(IInteractable interactable)
    {
        if (!nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }
    
    /// <summary>
    /// Menghapus interactable dari range
    /// </summary>
    public void RemoveNearbyInteractable(IInteractable interactable)
    {
        if (nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Remove(interactable);
            
            if (currentInteractable == interactable)
            {
                ClearCurrentInteractable();
            }
        }
    }
      private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        
        // Visualisasi berdasarkan detection method
        switch (detectionMethod)
        {
            case InteractionMethod.Raycast:
                // Raycast visualization
                Gizmos.color = Color.yellow;
                Vector3 rayStart = raycastOrigin.position + Vector3.up * 0.5f;
                Vector3 rayEnd = rayStart + raycastOrigin.forward * interactionRange;
                Gizmos.DrawLine(rayStart, rayEnd);
                Gizmos.DrawWireCube(rayEnd, Vector3.one * 0.2f);
                break;
                
            case InteractionMethod.Sphere:
                // Sphere visualization
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(raycastOrigin.position, sphereDetectionRadius);
                break;
                
            case InteractionMethod.Both:
                // Both visualizations
                Gizmos.color = Color.yellow;
                Vector3 rayStart2 = raycastOrigin.position + Vector3.up * 0.5f;
                Vector3 rayEnd2 = rayStart2 + raycastOrigin.forward * interactionRange;
                Gizmos.DrawLine(rayStart2, rayEnd2);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(raycastOrigin.position, sphereDetectionRadius);
                break;
        }
    }
}
