#if DEBUG
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathHamCore.Utility.WrathTools;

namespace WrathScalingItemDCs.ScalingDC.Debug;

internal class FindAllItems
{
    public static void Dump()
    {
        List<SimpleBlueprint> bps;

        while ((bps = BlueprintLoader.Shared.GetBlueprints()) == null) { }

        int count = 0;

        foreach (var bp in bps.OfType<BlueprintItemEquipment>().OrderBy(x => x.Name))
        {
            try
            {
                if (bp is BlueprintItemEquipmentUsable usable &&
                    (
                        usable.Type == UsableItemType.Wand ||
                        usable.Type == UsableItemType.Scroll ||
                        usable.Type == UsableItemType.Potion
                    ))
                    continue;

                if (ScalingDCModel.TryCreate(bp, out var scalingDCModel))
                {
                    Main.Logger.Debug($"{bp.Name} {bp.name}: {bp.AssetGuidThreadSafe}");
                    ScalingDCCollection.Instance.Add(scalingDCModel.ItemAssetId, scalingDCModel);
                    count++;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error: {bp.AssetGuid}");
                Main.Logger.Error(ex);
            }
        }

        ScalingDCCollection.Instance.Save();

        Main.Logger.Log($"Found {count} instances.");
    }
}
#endif