using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WrathHamCore.Utility.Initialization;

namespace WrathScalingItemDCs.ScalingDC.CSAPM;
internal class CSAPMCollection
{
    public const string FILENAME = "CSAPM_Collection.json";

    private Dictionary<string, CSAPMData> _moddedBlueprints = [];
    public IReadOnlyList<CSAPMData> ModdedBlueprints => _moddedBlueprints.Values.ToList().AsReadOnly();

    private CSAPMCollection() { }

    private static CSAPMCollection _instance;
    public static CSAPMCollection Instance => _instance;

    [InitializeOnBlueprints(100)]
    public static void Initialize()
    {
        _instance = new CSAPMCollection();
        _instance.Load();
        _instance.ApplyMods();
    }

    private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.None,
    };

    public void Add<T>(string guid, CSAPMData data) where T : BlueprintItemEquipment
    {
        _moddedBlueprints.Add(guid, data as CSAPMData);
    }

    public void CreateAndAdd<T>(T blueprint, List<string> localizationKeys) where T : BlueprintItemEquipment
    {
        var guid = blueprint.AssetGuidThreadSafe;

        if (_moddedBlueprints.ContainsKey(guid))
        {
            _moddedBlueprints[guid].AddLocalizationKeys(localizationKeys);
            return;
        }

        var bp = CSAPMData.Create(blueprint);
        bp.AddLocalizationKeys(localizationKeys);

        _moddedBlueprints.Add(guid, bp);
    }

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
            _moddedBlueprints = JsonConvert.DeserializeObject<Dictionary<string, CSAPMData>>(json);
        }
        catch (Exception ex)
        {
            Main.Logger.Error(ex);
        }
    }

    public void ApplyMods() => _moddedBlueprints.Values.ForEach(x => x.ApplyMod());

    public void PrintModCollection()
    {
        foreach (var mod in _moddedBlueprints.Values)
            Main.Logger.Log(mod.ToString());
    }
}
