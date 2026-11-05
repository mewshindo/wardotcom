using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WarDotCom.Models;

namespace WarDotCom.Commands
{
    public class SpawnpointCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "spawnpoint";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => [];

        public List<string> Permissions => [];

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2) return;
            var uplayer = (UnturnedPlayer)caller;

            if (command[1] == "gungame")
            {
                if (command[0] == "add")
                {
                    Plugin.Instance.GameManager.ConfigManager.GunGameConfig.Spawnpoints.Add(new Spawnpoint(
                    (int)uplayer.Position.x,
                    (int)uplayer.Position.y,
                    (int)uplayer.Position.z,
                    (int)uplayer.Rotation
                    ));
                    Plugin.Instance.GameManager.ConfigManager.GunGameConfig.Save();
                }
                if (command[0] == "remove")
                {
                    var closest = Plugin.Instance.GameManager.ConfigManager.GunGameConfig.Spawnpoints.FirstOrDefault(s => Vector3.Distance(uplayer.Position, s.Position()) < 3f);
                    if(closest.Position() == Vector3.zero)
                    {
                        UnturnedChat.Say(caller, "No spawnpoints of this game nearby", Color.red);
                        return;
                    }
                    Plugin.Instance.GameManager.ConfigManager.GunGameConfig.Save();
                }
                if (command[0] == "show")
                {
                    foreach(var spawnpoint in Plugin.Instance.GameManager.ConfigManager.GunGameConfig.Spawnpoints)
                    {
                        var effect = new TriggerEffectParameters()
                        {
                            asset = Plugin.Instance.LightningBoltEffect,
                            position = spawnpoint.Position() + Vector3.down * 5,
                            relevantDistance = 4096,
                            shouldReplicate = true,
                            reliable = true,
                        };
                        effect.SetRelevantPlayer(uplayer.Player);
                        effect.SetDirection(Vector3.up);
                        EffectManager.triggerEffect(effect);
                    }
                }
            }
        }
    }
}
