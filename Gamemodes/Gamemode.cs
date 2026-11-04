using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;
using WarDotCom.Models;

namespace WarDotCom;
public abstract class Gamemode
{
    public abstract void StartGame(List<Player> participants);
    public abstract void EndGame();
    public abstract void OnTimeOfDayUpdated();
    public abstract void OnPlayerDied(Player dead, EDeathCause cause, ELimb limb, Player killer);
    public abstract void OnPlayerDamageRequested(DamagePlayerParameters parameters, Player killer);
    public abstract void OnPlayerConnected(Player player);
    public abstract void OnPlayerDisconnected(Player player);
    public virtual void ClearPlayerInventory(Player player)
    {
        #pragma warning disable CS0612
        player.channel.send("tellSlot", (ESteamCall)1, (ESteamPacket)15,
        [
            0,
            0,
            new byte[0]
        ]);
        player.channel.send("tellSlot", (ESteamCall)1, (ESteamPacket)15,
        [
            1,
            0,
            new byte[0]
        ]);

        for (byte page = 0; page < 6; page++)
        {
            if (page == PlayerInventory.AREA)
                continue;

            var count = player.inventory.getItemCount(page);

            for (byte index = 0; index < count; index++)
            {
                player.inventory.removeItem(page, 0);
            }
        }

        player.clothing.askWearBackpack(0, 0, [], true);
        player.clothing.askWearGlasses(0, 0, [], true);
        player.clothing.askWearHat(0, 0, [], true);
        player.clothing.askWearPants(0, 0, [], true);
        player.clothing.askWearMask(0, 0, [], true);
        player.clothing.askWearShirt(0, 0, [], true);
        player.clothing.askWearVest(0, 0, [], true);

        for (byte i = 0; i < player.clothing.player.inventory.getItemCount(2); i++)
        {
            player.clothing.player.inventory.removeItem(2, 0);
        }
    }
    public virtual void ClearAllPlayers()
    {
        foreach (var sp in Provider.clients)
        {
            var player = sp.player;
            ClearPlayerInventory(player);
        }
    }
    public virtual void ClearPlayerGunSlots(Player player)
    {
#pragma warning disable CS0612
        player.channel.send("tellSlot", (ESteamCall)1, (ESteamPacket)15,
        [
            0,
            0,
            new byte[0]
        ]);
        player.channel.send("tellSlot", (ESteamCall)1, (ESteamPacket)15,
        [
            1,
            0,
            new byte[0]
        ]);

        for (byte page = 0; page < 2; page++)
        {
            if (page == PlayerInventory.AREA)
                continue;

            var count = player.inventory.getItemCount(page);

            for (byte index = 0; index < count; index++)
            {
                player.inventory.removeItem(page, 0);
            }
        }
    }
    public virtual void RevivePlayer(Player player, List<Spawnpoint> spawnpoints)
    {
        var spawnpoint = spawnpoints.RandomOrDefault();

        player.life.ServerRespawn(false);
        player.teleportToLocation(spawnpoint.Position, spawnpoint.Yaw);
    }
    public Action<Gamemode> OnGameFinished { get; set; }
}