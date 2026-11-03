using WarDotCom.Models;
using WarDotCom;
namespace WarDotCom;

public class GameManager
{
    private bool _instaRespawn = false;
    private PlayerUIManager _uiManager;
    private Player _punished;
    private IGameMode gamemode;

    public GameManager(Player punishedPlayer)
    {
        _uiManager = new PlayerUIManager();
        _punished = punishedPlayer;

        PlayerLife.onPlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
    {
        if(!_instaRespawn) return;

        if (_punished == null)
        {
            sender.ServerRespawn(false);
            return;
        }
        if (sender.player.channel.owner.playerID.steamID.m_SteamID != _punished.channel.owner.playerID.steamID.m_SteamID)
            sender.ServerRespawn(false);
    }

    public void Dispose()
    {
        PlayerLife.onPlayerDied -= OnPlayerDied;
    }
}