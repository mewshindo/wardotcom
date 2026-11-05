using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarDotCom.Gamemodes.GunGame;
using WarDotCom.States;

namespace WarDotCom.Commands
{
    public class StartGameCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "startgame";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => [];

        public List<string> Permissions => [];

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0) return;
            if (Plugin.Instance.GameManager.State is PlayingState) return;
            if (command[0].Trim().ToLower() == "gungame")
            {
                Plugin.Instance.GameManager.PlayGame(new GunGame(Plugin.Instance.GameManager.ConfigManager.GunGameConfig));
            }

        }
    }
}
