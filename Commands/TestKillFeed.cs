using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarDotComUI.Models;

namespace WarDotComUI.Commands;

public class TestKillFeed : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;

    public string Name => "testkillfeed";

    public string Help => "";

    public string Syntax => "";

    public List<string> Aliases => ["tkf"];

    public List<string> Permissions => [];

    public void Execute(IRocketPlayer caller, string[] command)
    {
        if(command.Length == 0) return;
        if (!int.TryParse(command[0], out int times)) return;

        for(; times > 0; times--)
        {
            KillMessage test = KillfeedTester.GenerateRandomKillMessage();
            Plugin.Instance.SendKillfeedMessage_Test(test);
        }
    }
}

public class KillfeedTester
{
    private static readonly Random rand = new Random();

    private static readonly string[] availableWeapons =
    {
    "WCOM_Desert_Falcon", "WCOM_Eaglestorm", "WCOM_Hawkhound",
    "WCOM_Colt", "WCOM_Bluntforce", "WCOM_Bastone",
    "WCOM_Natrix", "WCOM_Razortooth", "WCOM_Rhino",
    "WCOM_Thumber", "WCOM_Viper", "WCOM_Zubeknakov"
};

    private static readonly string[] sampleNames =
    {
    "porkchop78", "генстарl33tJAMER", "AlexTheGayFox", "uncle pussy", "Иван Купало"
};

    public static KillMessage GenerateRandomKillMessage()
    {
        string victimName = sampleNames[rand.Next(sampleNames.Length)];

        Array methods = Enum.GetValues(typeof(KillMethod));
        KillMethod randomMethod = (KillMethod)methods.GetValue(rand.Next(methods.Length));

        string killerName = null;
        string weapon = "";
        List<KillModifier> modifiers = null;

        switch (randomMethod)
        {
            case KillMethod.KnownGun:
                killerName = sampleNames[rand.Next(sampleNames.Length)];
                weapon = availableWeapons[rand.Next(availableWeapons.Length)];

                if (rand.NextDouble() <= 0.3)
                {
                    modifiers = new List<KillModifier> { KillModifier.Headshot };
                }
                break;

            case KillMethod.Melee:
            case KillMethod.Roadkill:
            case KillMethod.Explosion:
            case KillMethod.Admin:
                killerName = sampleNames[rand.Next(sampleNames.Length)];
                break;

            case KillMethod.Unknown:
            case KillMethod.Zombie:
            case KillMethod.Animal:
                killerName = null;
                break;
        }

        return new KillMessage(victimName, killerName, randomMethod, modifiers, weapon);
    }
}
