using UnityEngine;

public interface IInteractable
{
    bool CanInteract();
    
    void Interact(PlayerStateMachine player);
    
    string GetInteractionText();
    
    void OnLookAt();
    
    void OnLookAway();
}
