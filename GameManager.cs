using WarDotCom.UI;
using WarDotCom.States;
using SDG.Unturned;
using UnityEngine;
namespace WarDotCom;

public class GameManager
{
    private readonly PlayerUIManager _uiManager;
    public ConfigManager ConfigManager { get; private set; }
    public EventRouter EventRouter { get; private set; }
    public IGameState State { get; private set; }

    public GameManager()
    {
        _uiManager = new PlayerUIManager();

        EventRouter = new GameObject("WCOM_EventRouter").AddComponent<EventRouter>();
        EventRouter.Initialize(this);
        ConfigManager = new ConfigManager();
    }

    public void OnTimeOfDayTick()
    {
        if(State != null)
        {
            State.OnTimeOfDayUpdated();
        }
    }

    public void PlayGame(Gamemode gamemode)
    {
        State = new PlayingState(this, gamemode);
        State.OnEnter();
    }
}