using UnityEngine;


public class GameStartEvent : IGameEvent { }
public class GameEndEvent : IGameEvent { }

public class WaveStartEvent : IGameEvent
{
    public int WaveIndex;

    public WaveStartEvent(int waveIndex)
    {
        WaveIndex = waveIndex;
    }
}

public class WaveEndEvent : IGameEvent { }

public class TimerUpdateEvent : IGameEvent
{
    public int Timer;

    public TimerUpdateEvent(int timer)
    {
        Timer = timer;
    }
}

public class LevelSwitchOverEvent : IGameEvent
{
}