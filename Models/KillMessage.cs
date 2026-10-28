using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotComUI.Models;

public class KillMessage(ulong victim, ulong killer = 0, KillMethod method = KillMethod.Unknown, List<KillModifier> modifiers = null, string weapon = "")
{
    public readonly ulong Victim = victim;
    public readonly ulong Killer = killer;
    public readonly KillMethod Method = method;
    public readonly List<KillModifier> Modifiers = modifiers;
    public readonly string Weapon = weapon;
}