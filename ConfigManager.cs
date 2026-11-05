using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarDotCom.Gamemodes.GunGame;

namespace WarDotCom;

public class ConfigManager
{
    public GunGameConfig GunGameConfig { get; set; }

    public ConfigManager()
    {
        Console.WriteLine($"Creating config manager instance\nPlugin.Instance.Directory = {Plugin.Instance.Directory}");
        GunGameConfig = new GunGameConfig(Path.Combine(Plugin.Instance.Directory, "GunGameConfig.json"));
    }

    public void ReloadAll()
    {
        GunGameConfig.Load();
    }
}
