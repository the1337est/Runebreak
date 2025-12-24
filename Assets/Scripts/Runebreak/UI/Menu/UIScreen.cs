using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    private InputAction _backAction => GameManager.Instance.InputActions.UI.Back;
    
    private void OnEnable()
    {
        _backButton?.onClick.AddListener(HandleBackButtonClicked);
        GameManager.Instance.EnableInputActions(ActionMapType.UI);
        _backAction.performed += HandleBackAction;
        var firstButton = GetComponentInChildren<Button>();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    private void HandleBackAction(InputAction.CallbackContext obj)
    {
        HandleBackButtonClicked();
        
    }

    private void OnDisable()
    {
        _backButton?.onClick.RemoveListener(HandleBackButtonClicked);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void HandleBackButtonClicked()
    {
        gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventBus.Publish(new MenuScreenEnterEvent(MenuScreenID.Main));
    }
}
