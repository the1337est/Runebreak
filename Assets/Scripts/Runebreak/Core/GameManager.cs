using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private const string _menuSceneName = "Menu";
    private const string _gameSceneName = "Game";
    
    public static GameManager Instance { get; private set; }

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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
}
