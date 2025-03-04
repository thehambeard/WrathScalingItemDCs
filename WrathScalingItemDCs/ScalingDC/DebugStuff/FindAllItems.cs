#if DEBUG
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Shields;
using Kingmaker.Blueprints.Items.Weapons;
using System.Collections.Generic;
using System.Linq;
using WrathHamCore.Utility.Extensions;
using WrathHamCore.Utility.WrathTools;
using WrathScalingItemDCs.ScalingDC.CSAPM;

namespace WrathScalingItemDCs.ScalingDC.Debug;

internal class FindAllItems
{
    public static void Dump()
    {
        List<SimpleBlueprint> bps;

        while ((bps = BlueprintLoader.Shared.GetBlueprints()) == null) { }

        foreach (var bp in bps
            .Where(bp => bp is BlueprintItemWeapon 
                || bp is BlueprintItemArmor
                || bp is BlueprintItemShield)
            .Cast<BlueprintItemEquipment>())
            foreach (var enchant in bp.Enchantments)
                foreach (var csap in enchant.GetContextSetAbilityParams())
                    CSAPMCollection.Instance.CreateAndAdd(bp, GetValidKeys(enchant, bp));

        CSAPMCollection.Instance.Save();
    }

    private static List<string> GetValidKeys(BlueprintItemEnchantment enchantment, BlueprintItemEquipment item)
    {
        List<string> keys = [];

        if (!string.IsNullOrEmpty(enchantment.m_Description?.GetActualKey()))
            keys.Add(enchantment.m_Description.GetActualKey());

        if (!string.IsNullOrEmpty(item.m_DescriptionText?.GetActualKey()))
            keys.Add(item.m_DescriptionText.GetActualKey());

        return keys;
    }
}
#endif