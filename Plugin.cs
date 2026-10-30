using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using Steamworks;
using UnityEngine;
using WarDotComUI.Models;
using System.Collections;
using Rocket.Unturned.Player;
using WarDotComUI.UIHandlers;

namespace WarDotComUI;

public class Plugin : RocketPlugin
{
    public static Plugin Instance;

    private PlayerUIManager _uiManager;
    private EffectAsset _lightningBolt;
    private EffectAsset _boom;

    public const short UIKEY = 12562;
    protected override void Load()
    {
        Instance = this;
        Level.onLevelLoaded += onLevelLoaded;
        PlayerLife.onPlayerDied += OnPlayerDied;
        InteractableVehicle.OnFuelChanged_Global += OnFuelChanged_Global;
    }

    protected override void Unload()
    {
        Level.onLevelLoaded -= onLevelLoaded;
        PlayerLife.onPlayerDied -= OnPlayerDied;

        InteractableVehicle.OnFuelChanged_Global -= OnFuelChanged_Global;

        Instance = null;
    }

    private void OnFuelChanged_Global(InteractableVehicle obj)
    {
        if(obj.fuel <= 1)
        {
            VehicleManager.sendVehicleHealth(obj, 0);
        }
    }

    private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
    {
        var killerPlayer = UnturnedPlayer.FromCSteamID(instigator);
        if(killerPlayer != null)
        {
            if (cause == EDeathCause.GUN && limb == ELimb.SKULL)
            {
                EffectManager.sendUIEffectVisibility(UIKEY, killerPlayer.Player.channel.owner.transportConnection, true, "Canvas/SplashText_Container/Controller/Headshot", true);
            }
            else if (cause == EDeathCause.ROADKILL)
            {
                EffectManager.sendUIEffectVisibility(UIKEY, killerPlayer.Player.channel.owner.transportConnection, true, "Canvas/SplashText_Container/Controller/Roadkill", true);
            }
        }

        if (_punished == null) return;
        if (sender.player.channel.owner.playerID.steamID.m_SteamID != _punished.channel.owner.playerID.steamID.m_SteamID)
            sender.ServerRespawn(false);
    }

    private void onLevelLoaded(int level)
    {
        _uiManager = new PlayerUIManager();
        _lightningBolt = (EffectAsset)Assets.find(new Guid("bed12ffc45694cd69217924d75e96fe9"));
        _boom = (EffectAsset)Assets.find(new Guid("61d63a016a6448ffb6e4432a4c6a6ee1"));
    }

    public void Zeus(Player player)
    {
        EffectManager.sendUIEffectVisibility(UIKEY, player.channel.owner.transportConnection, true, "Canvas/Punish", true);
        player.movement.sendPluginSpeedMultiplier(0);
        player.movement.sendPluginJumpMultiplier(0);
        player.life.serverModifyHallucination(255);

        StartCoroutine(PunishCoroutine(player));
    }
    private IEnumerator PunishCoroutine(Player player)
    {
        yield return new WaitForSeconds(2);

        _punished = player;
        _zeusRounds = 8;
        _time = 0f;
    }

    private float _time = 0f;
    private byte _zeusRounds = 0;
    private Player _punished;
    private void Update()
    {
        if(_time > 0.3f && _zeusRounds > 0)
        {
            if(_punished == null)
            {
                _zeusRounds = 0;
                return;
            }
            var effect = new TriggerEffectParameters()
            {
                asset = _lightningBolt,
                position = _punished.transform.position,
                relevantDistance = 4096,
                shouldReplicate = true,
                reliable = true,
            };
            effect.SetDirection(Vector3.up);
            EffectManager.triggerEffect(effect);

            effect.asset = _boom;
            effect.position = _punished.transform.position + Vector3.down * 3;

            EffectManager.triggerEffect(effect);

            _time = 0f;
            _zeusRounds--;

            if(_zeusRounds == 0)
            {
                _punished.life.askDamage(101, Vector3.up * 101f, EDeathCause.KILL, ELimb.SKULL, Provider.server, out var _);
                _punished.movement.sendPluginSpeedMultiplier(1);
                _punished.movement.sendPluginJumpMultiplier(1);
                _punished = null;
            }
        }
        else
        {
            _time += Time.deltaTime;
        }
    }
}
