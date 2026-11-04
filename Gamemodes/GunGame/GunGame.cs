using SDG.Unturned;
using Steamworks;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarDotCom.Gamemodes.GunGame;

public class GunGame(GunGameConfig config) : Gamemode
{
    private GunGameConfig _config = config;
    private GunGameUI _gameUI;
    private float _timeRemaining;

    private List<ItemAsset> _weaponAssets;

    private List<GunGamePlayer> _players = [];
    public override void StartGame(List<Player> participants)
    {
        _timeRemaining = _config.DurationSeconds;

        EffectAsset ui = Assets.find(new Guid(_config.UIEffectGUID)) as EffectAsset 
            ?? throw new NullReferenceException($"Failed to find {this.GetType().Name} UI asset");

        List<ItemWeaponAsset> assets = [];
        Assets.find(assets);

        foreach(var weapon in _config.WeaponsInOrder)
        {
            var asset = assets.FirstOrDefault(w => w.name == weapon);
            if (asset != null)
            {
                _weaponAssets.Add(asset);
            }
        }

        foreach (var player in participants)
        {
            _players.Add(new GunGamePlayer
            {
                Player = player,
                CurrentWeapon = _weaponAssets[0].name,
                CSteamID = player.channel.owner.playerID.steamID.m_SteamID
            });
            player.life.askDamage(101, Vector3.up * 101f, EDeathCause.KILL, ELimb.SKULL, Provider.server, out var _);
            RevivePlayer(player, _config.Spawnpoints);
            UpdatePlayerLoadout(player);
        }

        _gameUI = new GunGameUI(ui, _players);
    }
    public override void OnTimeOfDayUpdated()
    {
        _timeRemaining--;
        _gameUI.UpdateClock(_timeRemaining);

        if(_timeRemaining == 0)
            EndGame();
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

        var player = _players.FirstOrDefault(p => p.Player == dead);
        if (player != null)
        {
            player.Score--;
        }

        var killerplayer = _players.FirstOrDefault(p => p.Player == killer);
        if (killerplayer != null)
        {
            killerplayer.Score++;

            killerplayer.Kills++;
            if(limb == ELimb.SKULL)
                killerplayer.Headshots++; 

            if (killerplayer.Score >= _config.ScoreToWin)
            {
                EndGame();
            }
        }
    }

    public override void OnPlayerDamageRequested(DamagePlayerParameters parameters, Player killer)
    {
        var index = _players.FindIndex(p => p.Player == killer);
        if(index != -1)
        {
            _players[index].DamageDealt += (ushort)parameters.damage;
        }
    }

    public override void OnPlayerConnected(Player player)
    {
        if (!_players.Any(p => p.Player == player))
        {
            _players.Add(new GunGamePlayer
            {
                Player = player,
                CurrentWeapon = _weaponAssets[0].name,
                CSteamID = player.channel.owner.playerID.steamID.m_SteamID
            });
        }
    }

    public override void OnPlayerDisconnected(Player player) { }

    private void UpdatePlayerLoadout(Player player)
    {
        var ggp = _players.FirstOrDefault(p => p.Player == player);
        if (ggp == null) return;

        int index = Math.Min(_weaponAssets.Count - 1, (int)Math.Ceiling(((float)_weaponAssets.Count / _config.ScoreToWin) * ggp.Score));

        ClearPlayerGunSlots(player);

        var item = new Item(_weaponAssets[index], EItemOrigin.ADMIN);
        player.inventory.tryAddItemAuto(item, true, true, true, true);
    }
}

public class GunGamePlayer
{
    public Player Player { get; set; }
    public string CurrentWeapon { get; set; }
    public ushort Kills {  get; set; }
    public ushort Score { get; set; }
    public ushort Deaths { get; set; }
    public ushort DamageDealt { get; set; }
    public ulong CSteamID { get; set; }
    public ushort Headshots { get; set; }
    public float HeadshotRate => (float)Kills / Headshots;
}
