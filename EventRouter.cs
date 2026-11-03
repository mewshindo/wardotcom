using Rocket.Unturned;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarDotCom.States;

namespace WarDotCom
{
    public class EventRouter
    {
        private readonly GameManager _gameManager;
        private bool Playing => _gameManager.State is PlayingState;

        public EventRouter(GameManager gameManager)
        {
            _gameManager = gameManager;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;

            PlayerLife.onPlayerDied += onPlayerDied;

        }

        private void onPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, Steamworks.CSteamID instigator)
        {
            if (!Playing) return;
        }

        private void OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            if (!Playing) return;
        }

        private void OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            if (!Playing) return;
        }
    }
}
