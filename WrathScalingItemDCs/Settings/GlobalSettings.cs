﻿using Newtonsoft.Json;
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
            Preset1 = .1f,
            Preset2 = .15f,
            Preset3 = .2f
        };

        [JsonProperty]
        public ScaleSettingFlat ScaleSettingFlat { get; set; } = new ScaleSettingFlat()
        {
            Preset1 = 3,
            Preset2 = 5,
            Preset3 = 7
        };
        
        [JsonProperty]
        public ScaleSettingDiminishingReturns ScaleSettingDiminishingReturns { get; set; } = new ScaleSettingDiminishingReturns()
        {
            Preset1 = (.24f, 9f, -3.7f),
            Preset2 = (.24f, 9f, -3.7f),
            Preset3 = (.24f, 9f, -3.7f)
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
                    else if (_instance.CurrentSettingType == typeof(ScaleSettingDiminishingReturns))
                        _instance.CurrentSetting = _instance.ScaleSettingDiminishingReturns;
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
            else if (_instance.CurrentSetting.GetType() == typeof(ScaleSettingDiminishingReturns))
                _instance.CurrentSettingType = typeof(ScaleSettingDiminishingReturns);

            var json = JsonConvert.SerializeObject(_instance, Formatting.Indented, _jsonSettings);
            File.WriteAllText(path, json);

            CSAPMCollection.Instance.ApplyMods();
        }
    }
}
