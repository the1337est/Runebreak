using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    [Header("Screen References")]
    [SerializeField] private UIScreen _creditsScreen;
    [SerializeField] private UIScreen _settingsScreen;
    
    [Header("Button References")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;
    
    [Header("Misc")]
    [SerializeField] TextMeshProUGUI _versionText;
    private const string _gameScene = "Game";
    private const string _menuScene = "Menu";

    private void Awake()
    {
        _versionText.text = $"v{Application.version}";
        GameManager.Instance.EnableInputActions(ActionMapType.UI);
    }

    private void OnDestroy()
    {
        GameManager.Instance.EnableInputActions(ActionMapType.Player);
    }

    private void OnEnable()
    {
        _startButton.onClick.AddListener(HandleStartButtonClicked);
        _creditsButton.onClick.AddListener(HandleCreditsButtonClicked);
        _settingsButton.onClick.AddListener(HandleSettingsButtonClicked);
        EventBus.Subscribe<MenuScreenEnterEvent>(HandleMenuScreenEnter);
        EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
    }
    
    private void OnDisable()
    {
        _startButton.onClick.AddListener(HandleStartButtonClicked);
        _creditsButton.onClick.AddListener(HandleCreditsButtonClicked);
        _settingsButton.onClick.AddListener(HandleSettingsButtonClicked);
        GameManager.Instance?.EnableInputActions(ActionMapType.Player);
        EventSystem.current?.SetSelectedGameObject(null);
        EventBus.Unsubscribe<MenuScreenEnterEvent>(HandleMenuScreenEnter);
    }
    
    private void HandleMenuScreenEnter(MenuScreenEnterEvent eventData)
    {
        if(eventData.ScreenID == MenuScreenID.Main)
        {
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
        }
    }
    
    private void HandleStartButtonClicked()
    {
        SceneManager.LoadScene(_gameScene);
    }

    private void HandleSettingsButtonClicked()
    {
        _settingsScreen.Show();
    }

    private void HandleCreditsButtonClicked()
    {
        _creditsScreen.Show();
    }
}
