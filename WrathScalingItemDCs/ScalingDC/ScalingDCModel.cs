using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Shields;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.EquipmentEnchants;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WrathHamCore.Utility.WrathTools;
using WrathScalingItemDCs.Settings;

namespace WrathScalingItemDCs.ScalingDC
{
    [Serializable]
    public class ScalingDCModel
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string ItemAssetId { get; set; }

        [JsonProperty]
        public HashSet<string> LocalizationKeys { get; set; } = [];

        private BlueprintItemEquipment _blueprintItemEquipment;
        private readonly List<ContextSetAbilityParams> _contextSetAbilityParams = [];
        private readonly List<ContextActionSavingThrow> _contextActionSavingThrow = [];
        private int _abilityDC = -1;

        private readonly Dictionary<ContextSetAbilityParams, int> _originalCSAPDCs = [];
        private readonly Dictionary<ContextActionSavingThrow, int> _originalCASTDCs = [];
        private int _originalAblityDC = -1;

        
        [JsonIgnore]
        public bool IsValid => _contextSetAbilityParams.Count > 0 || _contextActionSavingThrow.Count > 0 || _abilityDC != -1;

        public ScalingDCModel() { }

        public void ApplyMod()
        {
            if (_blueprintItemEquipment == null)
            {
                _blueprintItemEquipment = BlueprintTools.GetBlueprint<BlueprintItemEquipment>(ItemAssetId);
                Create_Internal(_blueprintItemEquipment);

                if (!IsValid)
                    Main.Logger.Error($"Unable to create scaling data for {Name}");
            }

            try
            {
                for (int i = 0; i < _contextSetAbilityParams.Count; i++)
                    _contextSetAbilityParams[i].DC.Value = GlobalSettings.Instance.CurrentSetting.GetModifier(_originalCSAPDCs[_contextSetAbilityParams[i]]);

                for (int i = 0; i < _contextActionSavingThrow.Count; i++)
                    _contextActionSavingThrow[i].CustomDC.Value = GlobalSettings.Instance.CurrentSetting.GetModifier(_originalCASTDCs[_contextActionSavingThrow[i]]);

                if (_abilityDC != -1)
                    _abilityDC = GlobalSettings.Instance.CurrentSetting.GetModifier(_originalAblityDC);
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex);
            }
        }

        public string ReplaceDCString(string input)
        {
            string pattern = @"(\{g\|Encyclopedia:DC\}..{/g} )(\d{1,2})";
            var regex = new Regex(pattern);

            var result = regex.Replace(input, match =>
            {
                string prefix = match.Groups[1].Value;
                int number = int.Parse(match.Groups[2].Value);

                return $"{prefix}{GlobalSettings.Instance.CurrentSetting.GetModifier(number)}";
            });

            return result;
        }


        public static bool TryCreate(BlueprintItemEquipment blueprintItemEquipment, out ScalingDCModel scalingDCModel, bool forceAllLocalKeys = false)
        {
            scalingDCModel = new()
            {
                Name = blueprintItemEquipment.name,
                ItemAssetId = blueprintItemEquipment.AssetGuidThreadSafe,
                _blueprintItemEquipment = blueprintItemEquipment
            };

            scalingDCModel.Create_Internal(blueprintItemEquipment, forceAllLocalKeys);

            if (blueprintItemEquipment is BlueprintItemShield shield)
            {
                if (shield.ArmorComponent != null)
                    scalingDCModel.Create_Internal(shield.ArmorComponent, forceAllLocalKeys);

                if (shield.WeaponComponent != null)
                    scalingDCModel.Create_Internal(shield.WeaponComponent, forceAllLocalKeys);
            }

            return scalingDCModel.IsValid;
        }

        private void Create_Internal(BlueprintItemEquipment blueprintItemEquipment, bool forceAllLocalKeys = false)
        {
            if (blueprintItemEquipment.Ability != null && blueprintItemEquipment.DC != 0)
            {
                _abilityDC = blueprintItemEquipment.DC;
                _originalAblityDC = blueprintItemEquipment.DC;
                ProcessComponents(blueprintItemEquipment.Ability.Components);
                if (HasDCStringPre(blueprintItemEquipment.Ability.Description) || forceAllLocalKeys)
                    LocalizationKeys.Add(blueprintItemEquipment.Ability.m_Description.GetActualKey());
            }

            if (blueprintItemEquipment.ActivatableAbility != null && blueprintItemEquipment.ActivatableAbility.Buff != null)
            {
                ProcessComponents(blueprintItemEquipment.ActivatableAbility.Buff.Components);
                if (HasDCStringPre(blueprintItemEquipment.ActivatableAbility.Description) || forceAllLocalKeys)
                    LocalizationKeys.Add(blueprintItemEquipment.ActivatableAbility.m_Description.GetActualKey());
            }

            if (blueprintItemEquipment.Enchantments == null)
                return;

            if (HasDCStringPre(blueprintItemEquipment.Description) || forceAllLocalKeys)
                LocalizationKeys.Add(blueprintItemEquipment.m_DescriptionText.GetActualKey());

            foreach (var enchant in blueprintItemEquipment.Enchantments)
            {
                if (HasDCStringPre(enchant.Description))
                    LocalizationKeys.Add(enchant.m_Description.GetActualKey());

                ProcessComponents(enchant.Components);
            }
        }

        private void ProcessComponents(BlueprintComponent[] components)
        {
            if (components == null)
                return;

            foreach (var comp in components)
            {
                switch (comp)
                {
                    case AddInitiatorAttackWithWeaponTrigger withWeaponTrigger:
                        if (withWeaponTrigger.Action != null && withWeaponTrigger.Action.HasActions)
                            ProcessActions(withWeaponTrigger.Action.Actions);
                        break;
                    case ContextSetAbilityParams contextSetAbilityParams:
                        if (contextSetAbilityParams.DC != null && contextSetAbilityParams.DC.Value > 0)
                        {
                            _contextSetAbilityParams.Add(contextSetAbilityParams);
                            _originalCSAPDCs[contextSetAbilityParams] = contextSetAbilityParams.DC.Value;
                        }
                        break;
                    case AddUnitFeatureEquipment addUnitFeatureEquipment:
                        if (addUnitFeatureEquipment.Feature != null)
                            ProcessComponents(addUnitFeatureEquipment.Feature.Components);
                        break;
                    case AddUnitFactEquipment addUnitFactEquipment:
                        if (addUnitFactEquipment.Blueprint != null)
                            ProcessComponents(addUnitFactEquipment.Blueprint.Components);
                        break;
                    case AbilityEffectRunAction abilityEffectRunAction:
                        if (abilityEffectRunAction.Actions.HasActions)
                            ProcessActions(abilityEffectRunAction.Actions.Actions);
                        break;
                    case CombatStateTrigger combatStateTrigger:
                        if (combatStateTrigger.CombatStartActions.HasActions)
                            ProcessActions(combatStateTrigger.CombatStartActions.Actions);
                        if (combatStateTrigger.CombatEndActions.HasActions)
                            ProcessActions(combatStateTrigger.CombatEndActions.Actions);
                        break;
                    case AddAreaEffect addAreaEffect:
                        if (addAreaEffect.AreaEffect != null)
                            ProcessComponents(addAreaEffect.AreaEffect.Components);
                        break;
                    case AddFactContextActions addFactContextActions:
                        if (addFactContextActions.Activated.HasActions)
                            ProcessActions(addFactContextActions.Activated.Actions);
                        break;
                    case AddTargetAttackRollTrigger addTargetAttackRoll:
                        if (addTargetAttackRoll.ActionsOnAttacker.HasActions)
                            ProcessActions(addTargetAttackRoll.ActionsOnAttacker.Actions);
                        break;
                    case BuffEnchantWornItem buffEnchantWornItem:
                        if (buffEnchantWornItem.Enchantment != null)
                            ProcessComponents(buffEnchantWornItem.Enchantment.Components);
                        break;
                    case AddTargetAttackWithWeaponTrigger addTargetAttackWithWeaponTrigger:
                        if (addTargetAttackWithWeaponTrigger.ActionOnSelf.HasActions)
                            ProcessActions(addTargetAttackWithWeaponTrigger.ActionOnSelf.Actions);
                        if (addTargetAttackWithWeaponTrigger.ActionsOnAttacker.HasActions)
                            ProcessActions(addTargetAttackWithWeaponTrigger.ActionsOnAttacker.Actions);
                        break;
                    case AbilityAreaEffectBuff abilityAreaEffectBuff:
                        if (abilityAreaEffectBuff.Buff != null)
                            ProcessComponents(abilityAreaEffectBuff.Buff.Components);
                        break;
                    case AddAbilityUseTrigger addAbilityUseTrigger:
                        if (addAbilityUseTrigger.Action.HasActions)
                            ProcessActions(addAbilityUseTrigger.Action.Actions);
                        break;
                    case AbilityAreaEffectRunAction abilityAreaEffectRunAction:
                        if (abilityAreaEffectRunAction.UnitEnter.HasActions)
                            ProcessActions(abilityAreaEffectRunAction.UnitEnter.Actions);
                        if (abilityAreaEffectRunAction.UnitMove.HasActions)
                            ProcessActions(abilityAreaEffectRunAction.UnitMove.Actions);
                        if (abilityAreaEffectRunAction.UnitExit.HasActions)
                            ProcessActions(abilityAreaEffectRunAction.UnitExit.Actions);
                        if (abilityAreaEffectRunAction.Round.HasActions)
                            ProcessActions(abilityAreaEffectRunAction.Round.Actions);
                        break;

                }
            }
        }

        private void ProcessActions(GameAction[] actions)
        {
            if (actions == null)
                return;

            foreach (var action in actions)
            {
                switch (action)
                {
                    case Conditional conditional:
                        if (conditional.IfTrue.HasActions)
                            ProcessActions(conditional.IfTrue.Actions);
                        if (conditional.IfFalse.HasActions)
                            ProcessActions(conditional.IfFalse.Actions);
                        break;
                    case ContextActionApplyBuff contextActionApplyBuff:
                        if (contextActionApplyBuff.Buff != null)
                        ProcessComponents(contextActionApplyBuff.Buff.Components);
                        break;
                    case ContextActionCastSpell actionCastSpell:
                        if (actionCastSpell.Spell != null)
                            ProcessComponents(actionCastSpell.Spell.Components);
                        break;
                    case ContextActionSavingThrow contextActionSavingThrow:
                        if (contextActionSavingThrow.HasCustomDC && contextActionSavingThrow.CustomDC.Value > 0)
                        {
                            _contextActionSavingThrow.Add(contextActionSavingThrow);
                            _originalCASTDCs[contextActionSavingThrow] = contextActionSavingThrow.CustomDC.Value;
                        }
                        break;
                    case ContextActionOnRandomTargetsAround actionRandomTargets:
                        if (actionRandomTargets.Actions.HasActions)
                            ProcessActions(actionRandomTargets.Actions.Actions);
                        break;
                }
            }
        }

        public static bool HasDCStringPre(string input)
        {
            var pattern = @"<link=""Encyclopedia:DC"">DC</link></color></b> \d{1,2}";
            return Regex.IsMatch(input, pattern);
        }
    }
}
