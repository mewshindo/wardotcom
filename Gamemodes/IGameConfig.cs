using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarDotCom.Wrappers;

namespace WarDotCom.Gamemodes
{
    public interface IGameConfig
    {
        float Duration { get; }
        List<Vector3Wrapper> Spawnpoints { get; }
        void Save();
        void Load();
    }
}
