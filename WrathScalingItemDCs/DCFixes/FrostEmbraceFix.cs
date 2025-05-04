using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrathHamCore.Utility.WrathTools;

namespace WrathScalingItemDCs.DCFixes
{
    internal class FrostEmbraceFix : IFix
    {
        public void Apply()
        {
            var bp = BlueprintTools.GetBlueprint<BlueprintWeaponEnchantment>("aa6b5b5557bb2824cb8ad4173eae8eea");
            var conditional = bp.GetComponent<AddInitiatorAttackWithWeaponTrigger>().Action.Actions[0] as Conditional;
            var context = conditional.IfFalse.Actions[0] as ContextActionSavingThrow;
            context.CustomDC.Value = 24;
        }
    }
}
