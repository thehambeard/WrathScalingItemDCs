using Kingmaker.Blueprints.Items.Equipment;

namespace WrathScalingItemDCs.ScalingDC
{
    public class ScalingDCAPI
    {
        public static void AddItem(BlueprintItemEquipment blueprintItemEquipment) =>
            ScalingDCCollection.Instance.AddFromExternalMod(blueprintItemEquipment);
    }
}
