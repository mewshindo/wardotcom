using System;

namespace WarDotCom;
public abstract class Gamemode
{
    public abstract void StartGame();
    public abstract void EndGame();
    public abstract void OnTick(float deltaTime);
    Action<Gamemode> onGameFinished { get; set; }
}