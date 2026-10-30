using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotComUI.Commands;

public class PunishCommand : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;

    public string Name => "punish";

    public string Help => "";

    public string Syntax => "";

    public List<string> Aliases => [];

    public List<string> Permissions => [];

    public void Execute(IRocketPlayer caller, string[] command)
    {
        if(command.Length == 0) return;

        var target = UnturnedPlayer.FromName(command[0]);
        if(target == null) return;

        Plugin.Instance.Zeus(target.Player);
    }
}
