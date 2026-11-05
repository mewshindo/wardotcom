using Rocket.Unturned.Player;
using Rocket.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace WarDotCom.UI
{
    public class PlayerUIManager
    {
        private readonly Dictionary<UnturnedPlayer, LifeUI> _lifeUIs = [];

        public static Dictionary<ulong, string> CSteamIDAvatarCache = [];

        private readonly Killfeed _killfeed;

        private readonly EffectAsset _killFeedUIEffect;
        private readonly EffectAsset _lifeUIEffect;

        public PlayerUIManager()
        {
            var effect = (EffectAsset)Assets.find(new Guid("4a012d9f20e74735a303fdc2b0738d40"));

            if (effect != null)
                _killFeedUIEffect = effect;

            effect = (EffectAsset)Assets.find(new Guid("c788c172149846fbb1d698d9f234b0f2"));

            if (effect != null)
                _lifeUIEffect = effect;

            _killfeed = new GameObject("Killfeed").AddComponent<Killfeed>();

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            _lifeUIs[player].Dispose();
            _lifeUIs.Remove(player);
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            TryCachePlayerAvatar(player);
            EffectManager.SendUIEffect(_lifeUIEffect, Plugin.UIKEY + 1, player.Player.channel.owner.transportConnection, true);
            EffectManager.SendUIEffect(_killFeedUIEffect, Plugin.UIKEY, player.Player.channel.owner.transportConnection, true);

            _lifeUIs.Add(player, new LifeUI(player));
        }

        private async void TryCachePlayerAvatar(UnturnedPlayer player)
        {
            if (CSteamIDAvatarCache.ContainsKey(player.CSteamID.m_SteamID)) return;

            var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key=4F8ACDBD828AFF3693B9E61814EC85D3&steamids={player.CSteamID.m_SteamID}";

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
                Console.WriteLine($"Failed to get Steam Avatar for {player.CSteamID.m_SteamID}: {ex.Message}");
            }

            EffectManager.sendUIEffectImageURL(Plugin.UIKEY + 1, player.Player.channel.owner.transportConnection, true, "SteamProfilePicture", avatarUrl);

            CSteamIDAvatarCache.Add(player.CSteamID.m_SteamID, avatarUrl);
        }
    }
}
