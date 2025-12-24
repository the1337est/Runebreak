public class GameSceneEnterEvent : IGameEvent { }

public class MenuScreenEnterEvent : IGameEvent
{
    public MenuScreenID ScreenID;

    public MenuScreenEnterEvent(MenuScreenID screenID)
    {
        ScreenID = screenID;
    }
}

public enum MenuScreenID
{
    Main,
    Settings,
    Credits
}
