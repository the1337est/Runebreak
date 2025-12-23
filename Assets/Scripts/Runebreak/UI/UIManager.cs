using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private Button _returnToMenuButton;
    
    [Header("Pause Menu UI Components")]
    [SerializeField] private GameObject _pauseMenuContainer;
    [SerializeField] private Button _resumeButton;
    
    [SerializeField] private WaveEndScreen _waveEndScreen;
    [SerializeField] private GameFinishedScreen _gameFinishedScreen;
    
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
        _returnToMenuButton.onClick.AddListener(HandleReturnToMenuClicked);
        
        EventBus.Subscribe<TimerUpdateEvent>(HandleTimerUpdate);
        EventBus.Subscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Subscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Subscribe<GameEndEvent>(HandleGameEnd);
        
        EventBus.Subscribe<PlayerDeathEvent>(HandlePlayerDeath);
        EventBus.Subscribe<ReturnToMenuEvent>(HandleReturnToMenu);
    }

    private void OnDisable()
    {
        _returnToMenuButton.onClick.RemoveListener(HandleReturnToMenuClicked);
        
        EventBus.Unsubscribe<TimerUpdateEvent>(HandleTimerUpdate);
        EventBus.Unsubscribe<WaveStartEvent>(HandleWaveStart);
        EventBus.Unsubscribe<WaveEndEvent>(HandleWaveEnd);
        EventBus.Unsubscribe<GameEndEvent>(HandleGameEnd);
        
        EventBus.Unsubscribe<PlayerDeathEvent>(HandlePlayerDeath);
        EventBus.Unsubscribe<ReturnToMenuEvent>(HandleReturnToMenu);
    }

    private void HandleReturnToMenuClicked()
    {
        SceneManager.LoadScene("Menu");
    }
    
    private void HandleReturnToMenu(ReturnToMenuEvent eventData)
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
        _waveEndScreen.Show();
    }
    
    private void HandleGameEnd(GameEndEvent obj)
    {
        _gameFinishedScreen.Show();
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
