﻿using HarmonyLib;
using Kingmaker;
using Kingmaker.GameModes;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using System.Linq;
using WrathScalingItemDCs.ScalingDC.CSAPM;

namespace WrathScalingItemDCs.ScalingDC
{
    [HarmonyPatch]
    internal static class DescriptionPatch
    {
        [HarmonyPatch(typeof(LocalizedString), nameof(LocalizedString.LoadString))]
        [HarmonyPostfix]
        private static void PatchGetText(ref string __result, LocalizedString __instance, LocalizationPack pack, Locale locale)
        {
            var csapmData = CSAPMCollection.Instance?.ModdedBlueprints.Where(x => x.LocalizationKeys.Contains(__instance.GetActualKey())).FirstOrDefault();
            
            if (csapmData == null)
                return;

            __result = csapmData.ReplaceDCString(__result);
        }
    }
}
