using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WarDotCom.Models;

namespace WarDotCom.Gamemodes.GunGame
{
    public class GunGameConfig
    {
        public float DurationSeconds { get; private set; } = 300;
        public ushort ScoreToWin { get; private set; } = 20;
        public string UIEffectGUID { get; private set; } = "f1476a78dd8f42b9a29ac19561acfa09";
        public List<Spawnpoint> Spawnpoints { get; private set; } = [];
        public List<string> WeaponsInOrder { get; private set; } =
        [
            "WCOM_Thumber",
            "WCOM_Bastone",
            "WCOM_Bluntforce",
            "WCOM_Eaglestorm",
            "WCOM_Rhino",
            "WCOM_Zubeknakov",
            "WCOM_Hawkhound",
            "WCOM_Viper",
            "WCOM_Razortooth",
            "WCOM_Desert_Falcon",
            "WCOM_Natrix",
            "WCOM_Colt",
            "Knife_Military"
        ];

        private readonly string _savePath;
        public GunGameConfig(string savePath)
        {
            _savePath = savePath;
            Load();
        }

        public void Save()
        {
            if (_savePath == null) return;
            Console.WriteLine($"Tried saving to {_savePath}");
            string file = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_savePath, file);
        }
        public void Load()
        {
            if (_savePath == null) return;
            Console.WriteLine($"Tried loading from {_savePath}");
            string json;

            if (File.Exists(_savePath))
            {
                json = File.ReadAllText(_savePath);
            }
            else
            {
                string file = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(_savePath, file);

                json = File.ReadAllText(_savePath);
            }

            GunGameConfig config = JsonConvert.DeserializeObject<GunGameConfig>(json) ?? throw new JsonSerializationException();

            this.Spawnpoints = config.Spawnpoints;
            this.DurationSeconds = config.DurationSeconds;
            this.UIEffectGUID = config.UIEffectGUID;
            this.WeaponsInOrder = config.WeaponsInOrder;
            this.ScoreToWin = config.ScoreToWin;
        }
    }
}
