using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WarDotCom.Wrappers;

namespace WarDotCom.Gamemodes.GunGame
{
    public class GunGameConfig : IGameConfig
    {
        private string _savePath;

        public float Duration { get; private set; }
        public List<Vector3Wrapper> Spawnpoints {  get; private set; }
        public GunGameConfig(string savePath)
        {
            _savePath = savePath;
        }
        public void Save()
        {
            string file = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_savePath, file);
        }
        public void Load()
        {
            string json = File.ReadAllText(_savePath);
            if (json == null) throw new FileNotFoundException();

            GunGameConfig config = JsonConvert.DeserializeObject<GunGameConfig>(json);
            if (config == null) throw new JsonSerializationException();

            this.Spawnpoints = config.Spawnpoints;
            this.Duration = config.Duration;
        }
    }
}
