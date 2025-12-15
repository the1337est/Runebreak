using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-50)]
public class InteractablesManager : MonoBehaviour
{
    private List<Interactable> _interactables;
    
    private Vector3 _targetPosition => Player.Instance.Position;
    private InputAction _interactInput => _inputActions.Player.Interact;
    private InputActions _inputActions => GameManager.Instance.InputActions;
    
    private Interactable _bestInteractable;
    private void Awake()
    {
        _interactables = new List<Interactable>();
    }
    
    private void OnEnable()
    {
        _interactInput.performed += HandleInteract;
        EventBus.Subscribe<InteractableEnabledEvent>(HandleInteractableEnabled);
        EventBus.Subscribe<InteractableDisabledEvent>(HandleInteractableDisabled);
    }
    
    private void OnDisable()
    {
        _interactInput.performed -= HandleInteract;
        EventBus.Unsubscribe<InteractableEnabledEvent>(HandleInteractableEnabled);
        EventBus.Unsubscribe<InteractableDisabledEvent>(HandleInteractableDisabled);
    }

    private void HandleInteractableEnabled(InteractableEnabledEvent eventData)
    {
        if (_interactables.Contains(eventData.Interactable)) return;
        _interactables.Add(eventData.Interactable);
    }
    
    private void HandleInteractableDisabled(InteractableDisabledEvent eventData)
    {
        if (_interactables.Contains(eventData.Interactable))
        {
            _interactables.Add(eventData.Interactable);
        }
    }

    private void Update()
    {
        ProximityUpdate();
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (_bestInteractable == null) return;
        _bestInteractable.Interact();
    }
    
    private void ProximityUpdate()
    {
        float bestDistance = float.MaxValue;
        _bestInteractable = null;
        foreach (var interactable in _interactables)
        {
            if(!interactable.InteractionAllowed) continue;
            if (!interactable.IsWithinRange(_targetPosition, out var distance)) continue;
            if (!(distance < bestDistance)) continue;
            
            bestDistance = distance;
            _bestInteractable = interactable;
        }
        
        foreach (var interactable in _interactables)
        {
            interactable.UpdateInteractable(interactable == _bestInteractable);
        }
    }
}
