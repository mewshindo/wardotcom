using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steamworks;
using Rocket.Unturned.Player;
using WarDotComUI.Models;
using System.Collections;
using Rocket.Unturned;

namespace WarDotComUI;

public class Killfeed : MonoBehaviour
{
    const float MESSAGE_LIFETIME_SECONDS = 10f;
    const float MESSAGE_FADEOUT_SECONDS = 1f;
    const float MESSAGE_APPEAR_COOLDOWN = 0.5f;
    const byte KILLFEED_SLOTS = 8;

    private readonly Queue<KillMessage> _queue = [];
    private float _time = 0f;
    private readonly List<KillMessage> _aliveMessages = [];

    private byte _lastDisplayedMessages;

    private void Awake()
    {
        PlayerLife.onPlayerDied += OnPlayerDied;
        DamageTool.damageAnimalRequested += OnDamageAnimalRequested;
    }

    private void Update()
    {
        if(_queue.Count > 0 && _time >= MESSAGE_APPEAR_COOLDOWN)
        {
            _time = 0f;

            _aliveMessages.Insert(0, _queue.Dequeue());
            UpdateKillMessages();
            StartCoroutine(MessageLifetimeCoroutine());
        }
        else
        {
            _time += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        PlayerLife.onPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
    {
        if (_aliveMessages.Count >= KILLFEED_SLOTS) return;
        if (sender == null) return;

        string victim = sender.player.channel.owner.playerID.characterName;
        if (instigator.m_SteamID == 0 || sender.player.channel.owner.playerID.steamID == instigator || instigator == Provider.server)
        {
            if (cause == EDeathCause.ZOMBIE)
            {
                var message = new KillMessage(victim, method: KillMethod.Zombie);
                EnqueueMessage(message);
            }
            else if (cause == EDeathCause.ANIMAL)
            {
                var message = new KillMessage(victim, method: KillMethod.Animal);
                EnqueueMessage(message);
            }
            else if (cause == EDeathCause.KILL)
            {
                var message = new KillMessage(victim, method: KillMethod.Admin);
                EnqueueMessage(message);
            }
            else if (cause == EDeathCause.SUICIDE)
            {
                var message = new KillMessage(victim, method: KillMethod.Suicide);
                EnqueueMessage(message);
            }
            else if (cause == EDeathCause.BLEEDING)
            {
                var message = new KillMessage(victim, method: KillMethod.Bleedout);
                EnqueueMessage(message);
            }
            else
            {
                var message = new KillMessage(victim, method: KillMethod.Unknown);
                EnqueueMessage(message);
            }
            return;
        }
        else
        {
            var killerPlayer = UnturnedPlayer.FromCSteamID(instigator);

            var message = new KillMessage(victim)
            {
                Killer = killerPlayer.CharacterName
            };

            if (cause == EDeathCause.GUN)
            {
                if (killerPlayer.Player.equipment.asset == null)
                {
                    message.Method = KillMethod.Unknown;
                }
                else
                {
                    var gun = killerPlayer.Player.equipment.asset;
                    if (gun is not ItemGunAsset || !gun.name.StartsWith("WCOM"))
                    {
                        message.Method = KillMethod.Unknown;
                    }
                    else
                    {
                        message.Method = KillMethod.KnownGun;
                        message.Weapon = gun.name;
                    }
                }
            }
            else if (cause == EDeathCause.MELEE || cause == EDeathCause.PUNCH)
            {
                message.Method = KillMethod.Melee;
            }
            else if (cause == EDeathCause.ROADKILL)
            {
                message.Method = KillMethod.Roadkill;
            }
            else if (cause == EDeathCause.GRENADE || cause == EDeathCause.MISSILE)
            {
                message.Method = KillMethod.Explosion;
            }

            if (limb == ELimb.SKULL)
            {
                message.Modifiers.Add(KillModifier.Headshot);
            }

            EnqueueMessage(message);
        }
    }

    private void OnDamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
    {
        if(parameters.damage >= parameters.animal.GetHealth())
        {
            if(parameters.instigator != null)
            {
                if (parameters.instigator is Player player)
                {
                    EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.channel.owner.transportConnection, true, "Huntsman", true);
                }
                else if (parameters.instigator is CSteamID id)
                {
                    EffectManager.sendUIEffectVisibility(Plugin.UIKEY, UnturnedPlayer.FromCSteamID(id).Player.channel.owner.transportConnection, true, "Huntsman", true);
                }
                else if (parameters.instigator is SteamPlayer sp)
                {
                    EffectManager.sendUIEffectVisibility(Plugin.UIKEY, sp.player.channel.owner.transportConnection, true, "Huntsman", true);
                }
            }
        }
    }

    private void EnqueueMessage(KillMessage message)
    {
        _queue.Enqueue(message);
    }

    private IEnumerator MessageLifetimeCoroutine()
    {
        yield return new WaitForSeconds(MESSAGE_LIFETIME_SECONDS + MESSAGE_FADEOUT_SECONDS);
        _aliveMessages.RemoveAt(_aliveMessages.Count - 1);
        Console.WriteLine($"Message expired");
        UpdateKillMessages();
    }

    private void UpdateKillMessages()
    {
        foreach (var player in Provider.clients)
        {
            for (int i = KILLFEED_SLOTS - 1; i >= _aliveMessages.Count; i--)
            {
                EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"Canvas/Container/Controller/Destroy_{i}", true);
            }
            for (int i = 0; i < _aliveMessages.Count; i++)
            {
                var message = _aliveMessages[i];
                SendKillMessage(player, message, i);
            }


            if (_aliveMessages.Count > 0 && (byte)_aliveMessages.Count > _lastDisplayedMessages)
                EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"Canvas/Container/Controller/Feed", true);

            _lastDisplayedMessages = (byte)_aliveMessages.Count;
        }
    }

    private void SendKillMessage(SteamPlayer player, KillMessage message, int index)
    {
        string prefix = $"Canvas/Container/Killfeed_Message_{index}";

        Console.WriteLine($"Sending Killfeed_Message_{index} to {player.player.name}");
        EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}", false);
        EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Kill_Modifiers", false);

        if (player.player.channel.owner.playerID.characterName == message.Victim || player.player.channel.owner.playerID.characterName == message.Killer && message.Method != KillMethod.Admin)
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Box_Outline", true);
        else if (message.Method != KillMethod.Admin)
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Box_Regular", true);

        EffectManager.sendUIEffectText(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Victim_Name", message.Victim);

        if (message.Method == KillMethod.Zombie     || 
            message.Method == KillMethod.Animal     || 
            message.Method == KillMethod.Admin      || 
            message.Method == KillMethod.Unknown    || 
            message.Method == KillMethod.Bleedout   || 
            message.Method == KillMethod.Suicide)
        {
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Kill_Methods/{message.Method}", true);

            if (message.Method == KillMethod.Admin)
            {
                EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Box_Outline_Gold", true);
            }
        }

        if (message.Killer == null)
        {
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}", true);
            return;
        }

        EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Killer_Name", true);
        EffectManager.sendUIEffectText(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Killer_Name", message.Killer);

        if (message.Method != KillMethod.KnownGun)
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Kill_Methods/{message.Method}", true);
        else
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Kill_Methods/{message.Weapon}", true);

        if (message.Modifiers != null)
        {
            EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Kill_Modifiers", true);
            foreach (var modifier in message.Modifiers)
            {
                EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}/Kill_Modifiers/{modifier}", true);
            }
        }

        EffectManager.sendUIEffectVisibility(Plugin.UIKEY, player.transportConnection, true, $"{prefix}", true);
    }

    public void EnqueueMessage_Test(KillMessage message)
    {
        EnqueueMessage(message);
    }
}
