using WarDotCom.UI;
using WarDotCom.States;
using SDG.Unturned;
namespace WarDotCom;

public class GameManager
{
    private readonly PlayerUIManager _uiManager;
    public IGameState State { get; private set; }

    public GameManager()
    {
        _uiManager = new PlayerUIManager();
    }

    public void OnTimeOfDayTick()
    {
        
    }
}