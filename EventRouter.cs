using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using WarDotCom.States;

namespace WarDotCom
{
    public class EventRouter
    {
        private readonly GameManager _gameManager;

        public EventRouter(GameManager gameManager)
        {
            _gameManager = gameManager;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            PlayerLife.onPlayerDied += onPlayerDied;
            LightingManager.onTimeOfDayChanged += OnTimeOfDayChanged;
        }
        public void Dispose()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            PlayerLife.onPlayerDied -= onPlayerDied;
            LightingManager.onTimeOfDayChanged -= OnTimeOfDayChanged;
        }

        private void OnTimeOfDayChanged()
        {
            _gameManager.OnTimeOfDayTick();
        }

        private void onPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            if (_gameManager.State is not PlayingState playingState) return;
            var killerPlayer = UnturnedPlayer.FromCSteamID(instigator);

            if (killerPlayer != null)
            {
                playingState.Gamemode.OnPlayerDied(sender.player, cause, limb, killerPlayer.Player);
            }
            else
            {
                playingState.Gamemode.OnPlayerDied(sender.player, cause, limb, null);
            }
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (_gameManager.State is not PlayingState playingState) return;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (_gameManager.State is not PlayingState playingState) return;
        }
    }
}
