using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class UIManager : MonoBehaviour
{
    [Header("Canvases")] 
    [SerializeField] private GameObject _gameplayCanvas;
    [SerializeField] private GameObject _pauseMenuCanvas;
    
    [Header("Gameplay UI Components")]
    [SerializeField] private GameObject _gameplayContainer;
    [SerializeField] private GameObject _initSetupContainer;
    [SerializeField] private GameObject _waveSetupContainer;
    [SerializeField] private GameObject _playerDeathContainer;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Button _nextWaveButton;

    [SerializeField] private Button _returnToMenuButton;
    
    [Header("Pause Menu UI Components")]
    [SerializeField] private GameObject _pauseMenuContainer;
    [SerializeField] private Button _resumeButton;
    
    
    public static UIManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _nextWaveButton.onClick.AddListener(HandleNextWaveClicked);
        _returnToMenuButton.onClick.AddListener(HandleReturnToMenuClicked);
        
        EventBus.Subscribe<TimerUpdateEvent>(HandleTimerUpdate);
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        
        EventBus.Subscribe<PlayerDeathEvent>(HandlePlayerDeath);
    }

    private void OnDisable()
    {
        _nextWaveButton.onClick.RemoveListener(HandleNextWaveClicked);
        _returnToMenuButton.onClick.RemoveListener(HandleReturnToMenuClicked);
        
        EventBus.Unsubscribe<TimerUpdateEvent>(HandleTimerUpdate);
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        
        EventBus.Unsubscribe<PlayerDeathEvent>(HandlePlayerDeath);
    }

    private void HandleReturnToMenuClicked()
    {
        SceneManager.LoadScene("Menu");
    }
    
    private void HandleNextWaveClicked()
    {
        _waveSetupContainer.SetActive(false);
        EventBus.Publish(new NextWaveClickEvent());
    }
    
    private void HandleWaveStart(WaveStartEvent eventData)
    {
        _waveText.text = $"Wave: {eventData.WaveIndex}";
    }
    
    private void HandleWaveEnd(WaveEndEvent eventData)
    {
        ResetText();
        _waveSetupContainer.SetActive(true);
    }

    private void HandleTimerUpdate(TimerUpdateEvent eventData)
    {
        _timerText.text = $"{eventData.Timer}";
    }

    private void HandlePlayerDeath(PlayerDeathEvent obj)
    {
        _playerDeathContainer.SetActive(true);
    }
    
    private void ResetText()
    {
        _timerText.text = "";
        _waveText.text = "";
    }
}
