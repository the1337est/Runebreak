using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{

    private const string _menuSceneName = "Menu";
    private const string _gameSceneName = "Game";
    
    public static GameManager Instance { get; private set; }

    public InputActions InputActions { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("instance created");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        InputActions = new InputActions();
        InputActions.Enable();
    }

    public void EnableInputActions(ActionMapType actionMapType)
    {
        switch (actionMapType)
        {
            case ActionMapType.Player:
                InputActions.Player.Enable();
                InputActions.UI.Disable();
                break;
            
            case ActionMapType.UI:
                InputActions.Player.Disable();
                InputActions.UI.Enable();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(actionMapType), actionMapType, null);
        }
        Debug.Log("EnableInputActions called");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventBus.Subscribe<GameStartEvent>(HandleGameStart);
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventBus.Unsubscribe<GameStartEvent>(HandleGameStart);
    }
    
    private void HandleGameStart(GameStartEvent obj)
    {
        EnableInputActions(ActionMapType.Player);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case _menuSceneName:
                EventBus.Publish(new MenuSceneEnterEvent());
                break;
            case _gameSceneName:
                EventBus.Publish(new GameSceneEnterEvent());
                break;
        }
    }

    private void OnDestroy()
    {
        InputActions.Disable();
    }
}

public enum ActionMapType
{
    Player,
    UI
}