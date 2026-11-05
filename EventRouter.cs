using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using WarDotCom.States;

namespace WarDotCom
{
    public class EventRouter : MonoBehaviour
    {
        private GameManager _gameManager;
        private uint _lastTime;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            PlayerLife.onPlayerDied += onPlayerDied;
            DamageTool.damagePlayerRequested += OnDamagePlayerRequested;
        }

        public void Dispose()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            PlayerLife.onPlayerDied -= onPlayerDied;
            DamageTool.damagePlayerRequested -= OnDamagePlayerRequested;

            Destroy(this);
        }

        private void Update()
        {
            if (_gameManager == null) return;
            if(LightingManager.time != _lastTime)
            {
                _gameManager.OnTimeOfDayTick();
                _lastTime = LightingManager.time;
            }
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

        private void OnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (_gameManager.State is not PlayingState playingState) return;
            var killerPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
            if (killerPlayer == null) return;

            playingState.Gamemode.OnPlayerDamageRequested(parameters, killerPlayer.Player);
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
