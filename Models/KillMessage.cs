using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotComUI.Models;

public class KillMessage(ulong victim, ulong killer = 0, KillMethod method = KillMethod.Unknown, List<KillModifier> modifiers = null, string weapon = "")
{
    public ulong Victim = victim;
    public ulong Killer = killer;
    public KillMethod Method = method;
    public List<KillModifier> Modifiers = modifiers;
    public string Weapon = weapon;
}