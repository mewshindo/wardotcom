using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace WarDotCom.Gamemodes.GunGame;

public class GunGame(GunGameConfig config) : Gamemode
{
    private GunGameConfig _config = config;
    private GunGameUI _gameUI;
    private float _timeRemaining;

    private Dictionary<Player, GunGamePlayer> _players = [];
    public override void StartGame(List<Player> participants)
    {
        _timeRemaining = _config.Duration;

        EffectAsset ui = Assets.find(new Guid(_config.UIEffectGUID)) as EffectAsset;
        if(ui == null)
            throw new NullReferenceException($"Failed to find {this.GetType().Name} UI asset");

        foreach(var player in participants)
        {
            _players.Add(player, new GunGamePlayer { CurrentWeapon = "" });
        }

        _gameUI = new GunGameUI(ui, _players);
    }
    public override void OnTimeOfDayUpdated()
    {
        _gameUI.UpdateClock(_timeRemaining);
    }
    public override void EndGame()
    {
        OnGameFinished?.Invoke(this);
        _players.Clear();
    }

    public override void OnPlayerDied(Player dead, EDeathCause cause, ELimb limb, Player killer)
    {
        RevivePlayer(dead, _config.Spawnpoints);

        if (killer == null) return;

        _gameUI.UpdateTopScoreboard();

        if (_players.TryGetValue(dead, out var player))
        {
            player.Score--;
        }

        if(_players.TryGetValue(killer, out var killerplayer))
        {
            killerplayer.Score++;

            if (killerplayer.Score >= _config.ScoreToWin)
            {
                EndGame();
            }
        }
    }

    public override void OnPlayerConnected(Player player)
    {
        if (!_players.ContainsKey(player))
            _players.Add(player, new GunGamePlayer { CurrentWeapon = "" });
    }

    public override void OnPlayerDisconnected(Player player)
    {
        if(_players.ContainsKey(player))
            _players.Remove(player);
    }
}

public struct GunGamePlayer
{
    public string CurrentWeapon { get; set; }
    public ushort Kills {  get; set; }
    public ushort Score { get; set; }
    public ushort Deaths { get; set; }
    public ushort DamageDealt { get; set; }
    public ulong CSteamID { get; set; }
}
