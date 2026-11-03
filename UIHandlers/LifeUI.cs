using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace WarDotComUI.UIHandlers;

public class LifeUI
{
    private readonly UnturnedPlayer _player;

    private readonly Dictionary<ushort, byte> ammoMaxes = [];

    private bool _staminaShown = true;
    public LifeUI(UnturnedPlayer player)
    {
        _player = player;

        PlayerLife.OnTellHealth_Global += OnHealthUpdated;
        PlayerLife.OnTellBroken_Global += OnBrokenUpdated;
        PlayerLife.OnTellBleeding_Global += OnBleedingUpdated;
        _player.Player.life.onStaminaUpdated += OnStaminaUpdated;
        _player.Player.movement.onRadiationUpdated += OnRadiationUpdated;
        _player.Player.life.onHurt += OnHurt;
        _player.Player.life.onLifeUpdated += OnLifeUpdated;
        PlayerEquipment.OnUseableChanged_Global += OnUseableChanged_Global;
        UseableGun.onBulletSpawned += onBulletSpawned;
        UseableGun.OnReloading_Global += OnReloading_Global;

        _player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowLifeMeters, false);
        _player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
        _player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus, false);
        _player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVehicleStatus, false);
        _player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowReputationChangeNotification, false);

        OnHealthUpdated(_player.Player.life);
        OnBrokenUpdated(_player.Player.life);
        OnBleedingUpdated(_player.Player.life);
        OnStaminaUpdated(_player.Stamina);
        OnRadiationUpdated(_player.Player.movement.isRadiated);
        OnUseableChanged_Global(_player.Player.equipment);

        GetUsenameAndProfilePicture();

        List<ItemMagazineAsset> assets = [];
        Assets.find(assets);
        foreach (ItemMagazineAsset mag in assets)
        {
            if (mag.name.StartsWith("WCOM"))
            {
                ammoMaxes.Add(mag.id, mag.amount);
            }
        }
    }

    private void OnHealthUpdated(PlayerLife life)
    {
        if (life.player.channel.owner.playerID.steamID.m_SteamID != _player.CSteamID.m_SteamID) return;

        SendUIText("Health_Percentage", $"{life.health}");

        var bar = new string('█', life.health);
        SendUIText("Bar_Health", bar);
    }

    private void OnReloading_Global(UseableGun obj)
    {
        if (obj.player.channel.owner.playerID.steamID.m_SteamID != _player.CSteamID.m_SteamID) return;

        UpdateAmmoUI();
    }

    private void OnBleedingUpdated(PlayerLife life)
    {
        if (life.player.channel.owner.playerID.steamID.m_SteamID != _player.CSteamID.m_SteamID) return;

        SendUIVisibility("Bleeding", life.isBleeding);
    }

    private void OnBrokenUpdated(PlayerLife life)
    {
        if (life.player.channel.owner.playerID.steamID.m_SteamID != _player.CSteamID.m_SteamID) return;

        SendUIVisibility("Bones", life.isBroken);
    }

    private void OnLifeUpdated(bool isDead)
    {
        SendUIVisibility("Canvas", !isDead);
        if (!isDead)
        {
            OnHealthUpdated(_player.Player.life);
            OnBrokenUpdated(_player.Player.life);
            OnBleedingUpdated(_player.Player.life);
            OnStaminaUpdated(_player.Stamina);
            OnRadiationUpdated(_player.Player.movement.isRadiated);
            OnUseableChanged_Global(_player.Player.equipment);
        }
    }

    private void OnHurt(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, Steamworks.CSteamID killer)
    {
        OnHealthUpdated(player.life);
    }

    private async void GetUsenameAndProfilePicture()
    {
        var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key=4F8ACDBD828AFF3693B9E61814EC85D3&steamids={_player.CSteamID.m_SteamID}";

        string avatarUrl = "";

        try
        {
            HttpClient httpClient = new();
            string response = await httpClient.GetStringAsync(url);
            JObject json = JObject.Parse(response);

            var players = json["response"]["players"];
            if (players.HasValues)
            {
                avatarUrl = players[0]["avatarfull"].ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get Steam Avatar for {_player.CSteamID.m_SteamID}: {ex.Message}");
        }

        EffectManager.sendUIEffectImageURL(Plugin.UIKEY + 1, _player.Player.channel.owner.transportConnection, true, "SteamProfilePicture", avatarUrl);

        SendUIText("Player_Name", _player.CharacterName);
    }

    private void onBulletSpawned(UseableGun gun, BulletInfo bullet)
    {
        if (gun.player.channel.owner.playerID.steamID.m_SteamID != _player.CSteamID.m_SteamID) return;

        var equipment = gun.player.equipment;

        if (equipment.asset == null)
        {
            ClearGunUI();
            return;
        }
        if (equipment.asset is not ItemGunAsset)
        {
            ClearGunUI();
            return;
        }
        if (!equipment.asset.name.StartsWith("WCOM"))
        {
            ClearGunUI();
            return;
        }

        UpdateAmmoUI();
    }

    private void SendUIText(string nameOrPath, string text)
    {
        EffectManager.sendUIEffectText(Plugin.UIKEY + 1, _player.Player.channel.owner.transportConnection, true, nameOrPath, text);
    }

    private void SendUIVisibility(string nameOrPath, bool visible)
    {
        EffectManager.sendUIEffectVisibility(Plugin.UIKEY + 1, _player.Player.channel.owner.transportConnection, true, nameOrPath, visible);
    }

    private void OnStaminaUpdated(byte newStamina)
    {
        if(newStamina >= 99 && _staminaShown)
        {
            SendUIVisibility("Stamina_OnOff", false);
            _staminaShown = false;
        }
        else if (newStamina < 99 && !_staminaShown)
        {
            SendUIVisibility("Stamina_OnOff", true);
            _staminaShown = true;
        }

        if (_staminaShown)
        {
            var bar = new string('█', newStamina);
            SendUIText("Bar_Stamina", bar);
        }
    }

    private void OnRadiationUpdated(bool isRadiated)
    {
        SendUIVisibility("Infection", isRadiated);
    }

    private void OnUseableChanged_Global(PlayerEquipment obj)
    {
        if (obj.player.channel.owner.playerID.steamID.m_SteamID != _player.CSteamID.m_SteamID) return;

        if (obj.asset == null)
        {
            ClearGunUI();
            return;
        }
        if (obj.asset is not ItemGunAsset)
        {
            ClearGunUI();
            return;
        }
        if (!obj.asset.name.StartsWith("WCOM"))
        {
            ClearGunUI();
            return;
        }

        UpdateGunUI();
    }

    private void UpdateGunUI()
    {
        SendUIVisibility("Gun_UI", true);
        SendUIVisibility(_player.Player.equipment.asset.name, true);

        UpdateAmmoUI();
    }
    private void UpdateAmmoUI()
    {
        var equipment = _player.Player.equipment;
        var ammo = equipment.state[10];

        bool shell = equipment.asset.name == "WCOM_Bluntforce" || equipment.asset.name == "WCOM_Bastone" || equipment.asset.name == "WCOM_Thumber";

        SendUIText("Ammo_Count", $"{ammo}/{ammoMaxes[BitConverter.ToUInt16(equipment.state, 8)]}");
        if (shell)
            SendUIVisibility("Shell", true);
        else
            SendUIVisibility("Bullet", true);
    }

    private void ClearGunUI()
    {
        SendUIVisibility("Gun_UI", false);
    }

    public void Dispose()
    {
        PlayerLife.OnTellHealth_Global -= OnHealthUpdated;
        PlayerLife.OnTellBroken_Global -= OnBrokenUpdated;
        PlayerLife.OnTellBleeding_Global -= OnBleedingUpdated;
        _player.Player.life.onStaminaUpdated -= OnStaminaUpdated;
        _player.Player.movement.onRadiationUpdated -= OnRadiationUpdated;
        _player.Player.life.onHurt -= OnHurt;
        _player.Player.life.onLifeUpdated -= OnLifeUpdated;
        PlayerEquipment.OnUseableChanged_Global -= OnUseableChanged_Global;
        UseableGun.onBulletSpawned -= onBulletSpawned;
        UseableGun.OnReloading_Global += OnReloading_Global;
    }
}
