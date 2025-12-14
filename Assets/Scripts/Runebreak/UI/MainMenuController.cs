using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;

    private const string _gameScene = "Game";
    
    private void OnEnable()
    {
        _startButton.onClick.AddListener(HandleStartButtonClicked);
        _creditsButton.onClick.AddListener(HandleCreditsButtonClicked);
        _settingsButton.onClick.AddListener(HandleSettingsButtonClicked);
    }
    
    private void OnDisable()
    {
        _startButton.onClick.AddListener(HandleStartButtonClicked);
        _creditsButton.onClick.AddListener(HandleCreditsButtonClicked);
        _settingsButton.onClick.AddListener(HandleSettingsButtonClicked);
    }
    
    private void HandleStartButtonClicked()
    {
        SceneManager.LoadScene(_gameScene);
    }

    private void HandleSettingsButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void HandleCreditsButtonClicked()
    {
        throw new NotImplementedException();
    }
    
    
}
