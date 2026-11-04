using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotCom.States;

public class PlayingState : IGameState
{
    private readonly GameManager _gameManager;

    public Gamemode Gamemode { get; private set; }

    public PlayingState(GameManager gameManager, Gamemode gamemode)
    {
        _gameManager = gameManager;
        Gamemode = gamemode;
    }

    public void OnEnter()
    {
        Gamemode.OnGameFinished += HandleGameFinished;
    }

    public void OnExit()
    {
        Gamemode.ClearAllPlayers();
    }

    private void HandleGameFinished(Gamemode gamemode)
    {
        throw new NotImplementedException();
    }

    public void OnTimeOfDayUpdated()
    {
        Gamemode.OnTimeOfDayUpdated();
    }
}
