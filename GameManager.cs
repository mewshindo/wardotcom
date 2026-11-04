using WarDotCom.UI;
using WarDotCom.States;
using SDG.Unturned;
namespace WarDotCom;

public class GameManager
{
    private readonly PlayerUIManager _uiManager;
    public ConfigManager ConfigManager { get; private set; }
    public IGameState State { get; private set; }

    public GameManager()
    {
        ConfigManager = new ConfigManager();
        _uiManager = new PlayerUIManager();
    }

    public void OnTimeOfDayTick()
    {
        
    }
}