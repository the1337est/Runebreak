using UnityEngine;

public class InteractableEnabledEvent : IGameEvent
{
    public Interactable Interactable;

    public InteractableEnabledEvent(Interactable interactable)
    {
        Interactable = interactable;
    }
}

public class InteractableDisabledEvent : IGameEvent
{
    public Interactable Interactable;

    public InteractableDisabledEvent(Interactable interactable)
    {
        Interactable = interactable;
    }
}

