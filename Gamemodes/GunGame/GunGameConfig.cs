using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using WarDotCom.Models;

namespace WarDotCom.Gamemodes.GunGame
{
    public class GunGameConfig
    {
        public float DurationSeconds { get; private set; } = 300;
        public ushort ScoreToWin { get; private set; } = 20;
        public string UIEffectGUID { get; private set; } = "";
        public List<Spawnpoint> Spawnpoints { get; private set; }
        public List<string> WeaponsInOrder { get; private set; } = new List<string>()
        {
            "WCOM_Bastone",
            "WCOM_Bluntforce"
        };

        private readonly string _savePath;
        public GunGameConfig(string savePath)
        {
            _savePath = savePath;
            Load();
        }

        public void Save()
        {
            string file = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_savePath, file);
        }
        public void Load()
        {
            string json = File.ReadAllText(_savePath) ?? throw new FileNotFoundException();

            GunGameConfig config = JsonConvert.DeserializeObject<GunGameConfig>(json) ?? throw new JsonSerializationException();

            this.Spawnpoints = config.Spawnpoints;
            this.DurationSeconds = config.DurationSeconds;
            this.UIEffectGUID = config.UIEffectGUID;
            this.WeaponsInOrder = config.WeaponsInOrder;
            this.ScoreToWin = config.ScoreToWin;
        }
    }
}
