using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float _activationRange = 4f;
    [SerializeField] private GameObject _enabledState;
    [SerializeField] private GameObject _disabledState;

    private Transform _target;
    private bool _withinRange;
    public bool InteractionAllowed { get; protected set; }

    private InputActions _inputActions => GameManager.Instance.InputActions;
    private InputAction _interactInput => _inputActions.Player.Interact;

    private void Awake()
    {
        _target = Player.Instance.transform;
    }
    
    protected virtual void OnEnable()
    {
        _interactInput.performed += HandleInteract;
        EventBus.Publish(new InteractableEnabledEvent(this));
    }
    
    protected virtual void OnDisable()
    {
        _interactInput.performed -= HandleInteract;
        EventBus.Publish(new InteractableDisabledEvent(this));
    }

    private void HandleInteract(InputAction.CallbackContext obj)
    {
        if (!_withinRange) return;
        Interact();
    }

    public virtual void Interact() { }

    public bool IsWithinRange(Vector3 pos, out float dist)
    {
        var toTarget = pos - transform.position;
        dist = toTarget.magnitude;
        return dist <= _activationRange;
    }

    public void UpdateInteractable(bool interactable)
    {
        _enabledState.SetActive(interactable);
        _disabledState.SetActive(!interactable);
    }
}
