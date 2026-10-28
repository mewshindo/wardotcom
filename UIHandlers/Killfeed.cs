using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Rocket.Unturned.Player;
using WarDotComUI.Models;
using System.Collections;
using SDG.NetTransport;
using Rocket.Unturned;

namespace WarDotComUI;

public class Killfeed : MonoBehaviour
{
    const float MESSAGE_LIFETIME_SECONDS = 5f;
    const float MESSAGE_FADEOUT_SECONDS = 0.5f;
    const byte KILLFEED_SLOTS = 8;
    const short UIKEY = 12562;

    private EffectAsset _ui;
    private readonly Queue<KillMessage> _aliveMessages = [];

    private void Awake()
    {
        var effect = (EffectAsset)Assets.find(new Guid(""));

        if (effect != null)
            _ui = effect;

        PlayerLife.onPlayerDied += OnPlayerDied;
        U.Events.OnPlayerConnected += OnPlayerConnected;
    }

    private void OnPlayerConnected(UnturnedPlayer player)
    {
        EffectManager.SendUIEffect(_ui, UIKEY, player.Player.channel.owner.transportConnection, true);
    }

    private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
    {
        if (_aliveMessages.Count >= KILLFEED_SLOTS) return;

        ulong victim = sender.player.channel.owner.playerID.steamID.m_SteamID;
        if (UnturnedPlayer.FromCSteamID(instigator) == null)
        {
            if (cause == EDeathCause.ZOMBIE)
            {
                var message = new KillMessage(victim, method: KillMethod.Zombie);
                EnqueueMessage(message);
            }
        }
    }

    private void EnqueueMessage(KillMessage message)
    {
        _aliveMessages.Enqueue(message);
        UpdateKillMessages();
        StartCoroutine(MessageLifetimeCoroutine());
    }

    private IEnumerator MessageLifetimeCoroutine()
    {
        yield return new WaitForSeconds(MESSAGE_LIFETIME_SECONDS + MESSAGE_FADEOUT_SECONDS);
        _aliveMessages.Dequeue();
        UpdateKillMessages();
    }

    private void UpdateKillMessages()
    {
        foreach(var player in Provider.clients)
        {

            for (byte i = 0; i < _aliveMessages.Count; i++)
            {
                var message = _aliveMessages.ElementAt(i);
                SendKillMessage(player, message, i);
            }

            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"Feed_{_aliveMessages.Count - 1}", true);
        }
    }

    private void SendKillMessage(SteamPlayer player, KillMessage message, byte index)
    {
        string prefix = $"Canvas/Container/Killfeed_Message_{index}";

        var victimPlayer = UnturnedPlayer.FromCSteamID(new CSteamID(message.Victim));
        if (victimPlayer == null)
            return;

        if(player.playerID.steamID.m_SteamID == message.Victim || player.playerID.steamID.m_SteamID == message.Killer && message.Method != KillMethod.Admin)
            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Box_Outline", true);
        else
            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Box_Regular", true);

        EffectManager.sendUIEffectText(UIKEY, player.transportConnection, true, $"{prefix}/Victim_Name", victimPlayer.CharacterName);

        if (message.Method == KillMethod.Zombie || message.Method == KillMethod.Animal || message.Method == KillMethod.Admin)
        {
            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Kill_Methods/{message.Method}", true);

            if(message.Method == KillMethod.Admin)
            {
                EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Box_Outline_Gold", true);
            }
            return;
        }

        var killerPlayer = UnturnedPlayer.FromCSteamID(new CSteamID(message.Killer));
        if (killerPlayer == null)
            return;

        EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Killer_Name", true);
        EffectManager.sendUIEffectText(UIKEY, player.transportConnection, true, $"{prefix}/Killer_Name", killerPlayer.CharacterName);

        if (message.Method != KillMethod.KnownGun)
            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Kill_Methods/{message.Method}", true);
        else
            EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Kill_Methods/{message.Weapon}", true);

        if (message.Modifiers != null)
        {
            foreach (var modifier in message.Modifiers)
            {
                EffectManager.sendUIEffectVisibility(UIKEY, player.transportConnection, true, $"{prefix}/Kill_Modifiers/{modifier}", true);
            }
        }
    }
}
