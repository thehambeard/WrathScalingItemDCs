using Newtonsoft.Json;
using System;
using System.IO;
using WrathHamCore.Utility.Initialization;
using UniRx;
using WrathScalingItemDCs.ScalingDC.CSAPM;
using System.CodeDom;

namespace WrathScalingItemDCs.Settings
{
    public class GlobalSettings
    {
        public const string FILENAME = "Settings.json";

        private static GlobalSettings _instance;
        public static GlobalSettings Instance
        {
            get
            {
                if (_instance == null)
                    throw new NullReferenceException("Settings has not been initialized!");

                return _instance;
            }
        }

        [InitializeOnLoad(100)]
        public static void Initialize()
        {
            Load();

            if (_instance.CurrentSetting == null)
                _instance.CurrentSetting = _instance.ScaleSettingPercent;
        }

        public GlobalSettings()
        {

        }

        [JsonProperty]
        public Type CurrentSettingType { get; set; } = typeof(ScaleSettingPercent);


        [JsonIgnore]
        public IScaleSetting CurrentSetting { get; set; }


        [JsonProperty]
        public ScaleSettingPercent ScaleSettingPercent { get; set; } = new ScaleSettingPercent()
        {
            Normal = .1f,
            Hard = .15f,
            Unfair = .2f
        };

        [JsonProperty]
        public ScaleSettingFlat ScaleSettingFlat { get; set; } = new ScaleSettingFlat()
        {
            Normal = 3,
            Hard = 5,
            Unfair = 7
        };

        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
        };

        public static void Load()
        {
            var path = Path.Combine(Main.ModEntry.Path, FILENAME);

            if (!File.Exists(path))
            {
                _instance = new GlobalSettings();
                return;
            }

            try
            {
                var json = File.ReadAllText(path);
                _instance = JsonConvert.DeserializeObject<GlobalSettings>(json);

                if (_instance != null)
                {
                    if (_instance.CurrentSettingType == typeof(ScaleSettingPercent))
                        _instance.CurrentSetting = _instance.ScaleSettingPercent;
                    else if (_instance.CurrentSettingType == typeof(ScaleSettingFlat))
                        _instance.CurrentSetting = _instance.ScaleSettingFlat;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex);
            }

            _instance ??= new GlobalSettings();
        }

        public void Save()
        {
            var path = Path.Combine(Main.ModEntry.Path, FILENAME);

            if (_instance.CurrentSetting.GetType() == typeof(ScaleSettingPercent))
                _instance.CurrentSettingType = typeof(ScaleSettingPercent);
            else if (_instance.CurrentSetting.GetType() == typeof(ScaleSettingFlat))
                _instance.CurrentSettingType = typeof(ScaleSettingFlat);

            var json = JsonConvert.SerializeObject(_instance, Formatting.Indented, _jsonSettings);
            File.WriteAllText(path, json);

            CSAPMCollection.Instance.ApplyMods();
        }
    }
}
