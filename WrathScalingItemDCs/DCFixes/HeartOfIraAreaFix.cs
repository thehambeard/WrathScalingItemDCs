using Kingmaker.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrathHamCore.Utility.WrathTools;

namespace WrathScalingItemDCs.DCFixes
{
    public class HeartOfIraAreaFix : IFix
    {
        public void Apply()
        {
            var bp = BlueprintTools.GetBlueprint<BlueprintAbilityAreaEffect>("3a636a7438e92a14386fc460f466be1b");
            var conditional = bp.GetComponent<AbilityAreaEffectRunAction>().Round.Actions[0] as Conditional;
            var context = conditional.IfFalse.Actions[1] as ContextActionSavingThrow;
            context.CustomDC.Value = 17;
        }
    }
}
