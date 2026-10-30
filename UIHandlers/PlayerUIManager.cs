using Rocket.Unturned.Player;
using Rocket.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;

namespace WarDotComUI.UIHandlers
{
    public class PlayerUIManager
    {
        private readonly Dictionary<UnturnedPlayer, LifeUI> _lifeUIs = [];
        private readonly Killfeed _killfeed;

        private readonly EffectAsset _killFeedUIEffect;
        private readonly EffectAsset _lifeUIEffect;

        public PlayerUIManager()
        {
            var effect = (EffectAsset)Assets.find(new Guid("4a012d9f20e74735a303fdc2b0738d40"));

            if (effect != null)
                _killFeedUIEffect = effect;

            effect = (EffectAsset)Assets.find(new Guid("4a012d9f20e74735a303fdc2b0738d40"));

            if (effect != null)
                _lifeUIEffect = effect;

            _killfeed = new GameObject("Killfeed").AddComponent<Killfeed>();

            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            _lifeUIs.Add(player, new LifeUI(player));
            EffectManager.SendUIEffect(_killFeedUIEffect, Plugin.UIKEY, player.Player.channel.owner.transportConnection, true);
            EffectManager.SendUIEffect(_lifeUIEffect, Plugin.UIKEY + 1, player.Player.channel.owner.transportConnection, true);



        }
    }
}
