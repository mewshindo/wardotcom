using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarDotComUI;

public class Plugin : RocketPlugin
{
    public static Plugin Instance;

    private Killfeed _killfeed;
    protected override void Load()
    {
        Instance = this;
        Level.onLevelLoaded += onLevelLoaded;
    }

    protected override void Unload()
    {
        Level.onLevelLoaded -= onLevelLoaded;
        Instance = null;
    }

    private void onLevelLoaded(int level)
    {
        _killfeed = new GameObject("Killfeed").AddComponent<Killfeed>();
    }
}
