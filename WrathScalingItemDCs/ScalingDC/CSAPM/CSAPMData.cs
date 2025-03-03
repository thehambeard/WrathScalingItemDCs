using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WrathHamCore.Utility.Extensions;
using WrathHamCore.Utility.WrathTools;
using WrathScalingItemDCs.Settings;

namespace WrathScalingItemDCs.ScalingDC.CSAPM;

[Serializable]
public class CSAPMData
{
    public CSAPMData() { }

    [JsonProperty]
    public string Name { get; set; }

    [JsonProperty]
    public string AssetId { get; set; }

    [JsonProperty]
    public HashSet<string> LocalizationKeys { get; set; } = [];

    [JsonIgnore]
    public BlueprintItemEquipment Blueprint { get; private set; }

    [JsonIgnore]
    public int OriginalBaseValue { get; private set; } = -1;

    [JsonIgnore]
    public int ModifiedBaseValue { get; private set; }

    public string ReplaceDCString(string input)
    {
        string pattern = @"(\{g\|Encyclopedia:DC\}..{/g} )(\d{1,2})";
        string replacement = $"${{1}}{ModifiedBaseValue}";

        return Regex.Replace(input, pattern, replacement);
    }

    public static CSAPMData Create(BlueprintItemEquipment blueprint, string bluprintPropertyGUID = "")
    {
        Main.Logger.Debug($"Creating mod data for {blueprint.name}");

        var guid = string.IsNullOrEmpty(bluprintPropertyGUID) ? Guid.NewGuid().ToString() : bluprintPropertyGUID;

        CSAPMData meta = new()
        {
            Blueprint = blueprint,
            Name = blueprint.name,
            AssetId = blueprint.AssetGuidThreadSafe,
        };

        return meta;
    }

    public void AddLocalizationKeys(List<string> keys) => LocalizationKeys.AddRange(keys);

    public void ApplyMod()
    {
        Main.Logger.Debug($"Scaling DC for {Name}: {AssetId}");

        if (Blueprint == null)
            Blueprint = BlueprintTools.GetBlueprint<BlueprintItemEquipment>(AssetId);

        if (Blueprint == null)
        {
            Main.Logger.Error($"Unable to get blueprint {Name}: {AssetId}");
            return;
        }

        foreach (var csap in Blueprint.GetContextSetAbilityParams())
        {
            if (OriginalBaseValue == -1)
                OriginalBaseValue = csap.DC.Value;

            ModifiedBaseValue = GlobalSettings.Instance.CurrentSetting.GetModifier(OriginalBaseValue);
            csap.DC.Value = ModifiedBaseValue;
        }
    }

    public override string ToString() =>
        $"Bluprint {Name}({AssetId}): Has {LocalizationKeys.Count} localization key(s)";
}
