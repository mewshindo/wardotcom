using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace WarDotComUI.UIHandlers;

public class LifeUI
{
    private readonly UnturnedPlayer _player;
    public LifeUI(UnturnedPlayer player)
    {
        _player = player;

        player.Player.life.onHealthUpdated += OnHealthUpdated;
        player.Player.life.onBleedingUpdated += OnBleedingUpdated;
        player.Player.life.onBrokenUpdated += OnBrokenUpdated;
        player.Player.life.onStaminaUpdated += OnStaminaUpdated;
        player.Player.movement.onRadiationUpdated += OnRadiationUpdated;
        PlayerEquipment.OnUseableChanged_Global += OnUseableChanged_Global;
        UseableGun.onBulletSpawned += onBulletSpawned;

        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowLifeMeters, false);
        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, false);
        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus, false);
        player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVehicleStatus, false);

        OnHealthUpdated(_player.Health);
        OnBleedingUpdated(_player.Bleeding);
        OnBrokenUpdated(_player.Broken);
        OnStaminaUpdated(_player.Stamina);
        OnRadiationUpdated(_player.Player.movement.isRadiated);
        OnUseableChanged_Global(_player.Player.equipment);

        GetUsenameAndProfilePicture();
    }

    private async Task GetUsenameAndProfilePicture()
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
        if (gun.player != _player.Player) return;

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

        var ammo = equipment.state[10];

        byte ammomax = Assets.find(EAssetType.ITEM, BitConverter.ToUInt16(equipment.state, 8)) is ItemMagazineAsset mag ? mag.amount : (byte)0;

        UpdateAmmoUI(ammo, ammomax, equipment.asset.name == "WCOM_Bluntforce" || equipment.asset.name == "WCOM_Baston" || equipment.asset.name == "WCOM_Thumber");
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
        var bar = new string('█', (int)(newStamina / 100f * 27f));
        SendUIText("Bar_Stamina", bar);
    }

    private void OnHealthUpdated(byte newHealth)
    {
        var bar = new string('█', (int)(newHealth / 100f * 27f));
        SendUIText("Bar_Health", bar);
    }

    private void OnRadiationUpdated(bool isRadiated)
    {
        SendUIVisibility("Infection", isRadiated);
    }

    private void OnBrokenUpdated(bool newBroken)
    {
        SendUIVisibility("Bones", newBroken);
    }

    private void OnBleedingUpdated(bool newBleeding)
    {
        SendUIVisibility("Bleeding", newBleeding);
    }

    private void OnUseableChanged_Global(PlayerEquipment obj)
    {
        if (obj.player != _player.Player) return;

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

        UpdateGunUI(obj);
    }

    private void UpdateGunUI(PlayerEquipment equipment)
    {
        var gunName = equipment.asset.name.Split(["COM_"], StringSplitOptions.None)[1];
        SendUIVisibility(gunName, true);

        var ammo = equipment.state[10];

        byte ammomax = Assets.find(EAssetType.ITEM, BitConverter.ToUInt16(equipment.state, 8)) is ItemMagazineAsset mag ? mag.amount : (byte)0;

        UpdateAmmoUI(ammo, ammomax, equipment.asset.name == "WCOM_Bluntforce" || equipment.asset.name == "WCOM_Baston" || equipment.asset.name == "WCOM_Thumber");
    }
    private void UpdateAmmoUI(byte ammo, byte ammoMax, bool shell)
    {
        SendUIText("Ammo_Count", $"{ammo}/{ammoMax}");
        if (shell)
            SendUIVisibility("Shell", true);
        else
            SendUIVisibility("Bullet", true);
    }

    private void ClearGunUI()
    {
        SendUIVisibility("Gun_UI", false);
    }
}
