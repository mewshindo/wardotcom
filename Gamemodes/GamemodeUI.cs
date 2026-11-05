using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace WarDotCom.Gamemodes
{
    public abstract class GamemodeUI
    {
        public const short UIKEY = Plugin.UIKEY + 2;

        public virtual void SendUI(Player player, EffectAsset ui)
        {
            EffectManager.SendUIEffect(ui, UIKEY, player.channel.owner.transportConnection, true);
        }
        public virtual void SendUI_AllPlayers(EffectAsset ui)
        {
            EffectManager.SendUIEffect(ui, UIKEY, true);
        }
        public virtual void SendUIText(SteamPlayer player, string nameOrPath, string text)
        {
            EffectManager.sendUIEffectText(UIKEY, player.transportConnection, true, nameOrPath, text);
        }
        public virtual void SendUIText(Player player, string nameOrPath, string text)
        {
            EffectManager.sendUIEffectText(UIKEY, player.channel.owner.transportConnection, true, nameOrPath, text);
        }
        public virtual void SendUIText_AllPlayers(string nameOrPath, string text)
        {
            foreach(var player in Provider.clients)
            {
                SendUIText(player, nameOrPath, text);
            }
        }
        public virtual void SendUIVisibility(SteamPlayer player, string nameOrPath, bool visible)
        {
            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, nameOrPath, visible);
        }
        public virtual void SendUIVisibility(Player player, string nameOrPath, bool visible)
        {
            EffectManager.sendUIEffectVisibility(UIKEY, player.channel.owner.transportConnection, true, nameOrPath, visible);
        }
        public virtual void SendUIVisibility_AllPlayers(string nameOrPath, bool visible)
        {
            foreach (var player in Provider.clients)
            {
                SendUIVisibility(player, nameOrPath, visible);
            }
        }
        public virtual void SendUIImage(SteamPlayer player, string nameOrPath, string url)
        {
            EffectManager.sendUIEffectImageURL(UIKEY, player.transportConnection, true, nameOrPath, url);
        }
        public virtual void SendUIImage(Player player, string nameOrPath, string url)
        {
            EffectManager.sendUIEffectImageURL(UIKEY, player.channel.owner.transportConnection, true, nameOrPath, url);
        }
        public virtual void SendUIImage_AllPlayers(string nameOrPath, string url)
        {
            foreach (var player in Provider.clients)
            {
                SendUIImage(player, nameOrPath, url);
            }
        }

        public virtual void ClearUI_AllPlayers(EffectAsset ui)
        {
            EffectManager.ClearEffectByGuid_AllPlayers(ui.GUID);
        }
        public virtual void ClearUI_AllPlayers(Guid guid)
        {
            EffectManager.ClearEffectByGuid_AllPlayers(guid);
        }
    }
}
