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
    private static HashSet<string> _badAssetIDs =
    [
        "371113ed05ba49feb78adf926ee3430b",
        "b25f68f32f074e6798bfe50bf3293c68",
        "4e82b54975834f14b81814e9255873e3",
        "372aae7b04ff4dd438ef3a8f881d5b17",
        "15acf7ada5903ec429f7cd62a6162613",
        "f96e68cd34d9f6340ad6a49e0477fce7",
        "a2206ab9a731411dbe5055ab48175a22",
        "a2bd66167b954e01b96add96585637e2",
        "401a40d73744474c9947a486451de7ea",
        "a2fe8669436c4168bd0084e96eb44f09",
        "7c30287784334774bdd0a971b5473744",
        "7c30287784334774bdd0a971b5473744",
        "15920d47a9e64669936e3c2030a6095d",
        "f548bd21606344f0963c0ae374946d39",
        "d390ae6685094218b1abd48919b73764",
        "d2d4b585db9f43698d183437f20bd8de",
        "7c861deccf5640f0a99bf0c034231465",
        "54cbc6480de54d49b277d2686a98b4fb",
        "2fe3c072b1054fbea6019c5b5db41403",
        "3f3aa97c9a1c46d5a8b3a3c22a5727db",
        "2de5a759c20544a0abaf04576e9e49f7",
        "8103f79802eb4ec59c373829ff5907a1",
        "6bc4f0bc278f48dd85821e3913ccffb5",
        "778b25032e7343ba856326bf7f3dfeab",
        "d0552e5dd29d49568cebfb8fd2335486",
        "4e98e92f49024c529cb2afa01fc63b0e",
        "2ede8fbd9b9a5784a8651f762e7c6f4e",
        "5b4f6f18b0664964f83705a163244283",
    ];

    public static void Dump()
    {
        List<SimpleBlueprint> bps;

        while ((bps = BlueprintLoader.Shared.GetBlueprints()) == null) { }

        int count = 0;

        

        foreach (var bp in bps
            .OfType<BlueprintItemEquipment>()
            .Where(x => !_badAssetIDs.Contains(x.AssetGuidThreadSafe))
            .OrderBy(x => x.Name))
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