using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WrathHamCore.Utility.Initialization;

namespace WrathScalingItemDCs.ScalingDC
{
    class ScalingDCCollection
    {
        public const string FILENAME = "ScaledItemsCollection.json";

        private Dictionary<string, ScalingDCModel> _moddedBlueprints = [];
        private Dictionary<string, ScalingDCModel> _queuedBlueprints = [];

        public IReadOnlyList<ScalingDCModel> ModdedBlueprints => _moddedBlueprints.Values.ToList().AsReadOnly();

        public bool IsLoaded { get; private set; } = false;

        private ScalingDCCollection() { }

        private static ScalingDCCollection _instance;
        public static ScalingDCCollection Instance => _instance;

        [InitializeOnBlueprints(100)]
        public static void Initialize()
        {
            _instance = new ScalingDCCollection();
            _instance.Load();
            _instance.ApplyMods();
        }

        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
        };

        public void Add(string guid, ScalingDCModel data) =>
            _moddedBlueprints.Add(guid, data);

        public void AddFromExternalMod(BlueprintItemEquipment blueprint)
        {
            if (ScalingDCModel.TryCreate(blueprint, out var newModel, true))
            {
                if (IsLoaded)
                    _moddedBlueprints.Add(blueprint.AssetGuidThreadSafe, newModel);
                else
                    _queuedBlueprints.Add(blueprint.AssetGuidThreadSafe, newModel);

                newModel.ApplyMod();
            }
        }

        public void ApplyMods() => _moddedBlueprints.Values.ForEach(x => x.ApplyMod());

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_moddedBlueprints, Formatting.Indented, _jsonSettings);
                File.WriteAllText(Path.Combine(Main.ModEntry.Path, FILENAME), json);
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex);
            }
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(Path.Combine(Main.ModEntry.Path, FILENAME)))
                    return;

                var json = File.ReadAllText(Path.Combine(Main.ModEntry.Path, FILENAME));
                _moddedBlueprints = JsonConvert.DeserializeObject<Dictionary<string, ScalingDCModel>>(json);

                if (_queuedBlueprints.Count > 0)
                    _queuedBlueprints.ForEach(x => _moddedBlueprints.Add(x.Key, x.Value));

                IsLoaded = true;
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex);
            }
        }
    }
}
