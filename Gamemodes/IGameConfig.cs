using System.Collections.Generic;
using WarDotCom.Models;

namespace WarDotCom.Gamemodes
{
    public interface IGameConfig
    {
        float Duration { get; }
        List<Spawnpoint> Spawnpoints { get; }
        void Save();
        void Load();
    }
}
