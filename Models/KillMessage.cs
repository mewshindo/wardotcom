using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotComUI.Models;

public class KillMessage(string victim, string killer = null, KillMethod method = KillMethod.Unknown, List<KillModifier> modifiers = null, string weapon = "")
{
    public string Victim = victim;
    public string Killer = killer;
    public KillMethod Method = method;
    public List<KillModifier> Modifiers = modifiers ?? [];
    public string Weapon = weapon;
}