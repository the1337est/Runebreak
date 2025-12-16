using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float _activationRange = 4f;
    [SerializeField] private GameObject _enabledState;
    [SerializeField] private GameObject _disabledState;
    
    public bool InteractionAllowed { get; protected set; }
    public bool IsActive { get; protected set; }
    
    protected virtual void OnEnable()
    {
        EventBus.Publish(new InteractableEnabledEvent(this));
    }
    
    protected virtual void OnDisable()
    {
        EventBus.Publish(new InteractableDisabledEvent(this));
    }

    public virtual void Interact()
    {
    }
    
    public virtual void UISelect()
    {
    }
    
    public virtual void UIBack()
    {
    }
    
    public virtual void UIOption()
    {
    }

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
