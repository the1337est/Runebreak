using UnityEngine;

public class StartGameClickEvent : IGameEvent
{
}

public class ReturnToMenuEvent : IGameEvent
{
}

public class NextWaveClickEvent : IGameEvent {}

public class PauseClickEvent : IGameEvent {}

public class ResumeClickEvent : IGameEvent {}