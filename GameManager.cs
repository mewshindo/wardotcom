using WarDotCom.Models;
using WarDotCom;
using WarDotCom.UI;
using SDG.Unturned;
using Steamworks;
using WarDotCom.States;
namespace WarDotCom;

public class GameManager
{
    private bool _instaRespawn = false;
    private PlayerUIManager _uiManager;
    private Gamemode gamemode;

    public IGameState State { get; private set; }

    public GameManager()
    {
        _uiManager = new PlayerUIManager();
    }
}