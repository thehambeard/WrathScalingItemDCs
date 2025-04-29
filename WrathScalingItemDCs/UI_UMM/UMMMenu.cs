using System;
using UnityModManagerNet;
using WrathHamCore.Utility.Initialization;
#if DEBUG
using WrathScalingItemDCs.ScalingDC.Debug;
#endif
using WrathScalingItemDCs.Settings;
using GL = UnityEngine.GUILayout;
namespace WrathScalingItemDCs.UI_UMM;

public static class UMMMenu
{
    private static Action _currentOnGUI;

    private static void SetOnGui(IScaleSetting scaleSetting)
    {
        GlobalSettings.Instance.CurrentSetting = scaleSetting;
        _currentOnGUI = scaleSetting switch
        {
            ScaleSettingPercent => OnGUIPercentBase,
            ScaleSettingFlat => OnGUIFlatBase,
            ScaleSettingDiminishingReturns => OnGUIDiminishReturn,
            _ => throw new NotImplementedException()
        };
    }

    public static void OnGUI(UnityModManager.ModEntry modEntry)
    {
        using (new GL.HorizontalScope())
        {
            if (GL.Button("Percent Based", GL.Width(200)))
                SetOnGui(GlobalSettings.Instance.ScaleSettingPercent);

            if (GL.Button("Flat Based", GL.Width(200)))
                SetOnGui(GlobalSettings.Instance.ScaleSettingFlat);

            if (GL.Button("Diminished Return", GL.Width(200)))
                SetOnGui(GlobalSettings.Instance.ScaleSettingDiminishingReturns);
        }



        if (_currentOnGUI == null)
            SetOnGui(GlobalSettings.Instance.CurrentSetting);

        _currentOnGUI?.Invoke();

        GL.Label("IMPORTANT: You must hit save even after changing your preset!");

        using (new GL.HorizontalScope())
        {
            if (GL.Button("Save", GL.Width(200)))
                GlobalSettings.Instance.Save();
            if (GL.Button("Revert", GL.Width(200)))
                GlobalSettings.Load();
        }
    }

    private delegate bool TryParseHandler<T>(string s, out T result);

    private static void OnGUIFlatBase()
    {


        using (new GL.VerticalScope())
        {
            GL.Label("Flat");
            GL.Label("This setting will add a flat modifier to the original DC");
            DrawDifficultySetting(ScaleSettingPreset.Preset1,
                GlobalSettings.Instance.ScaleSettingFlat,
                ref GlobalSettings.Instance.ScaleSettingFlat.Preset1,
                int.TryParse);
            DrawDifficultySetting(ScaleSettingPreset.Preset2,
                GlobalSettings.Instance.ScaleSettingFlat,
                ref GlobalSettings.Instance.ScaleSettingFlat.Preset2,
                int.TryParse);
            DrawDifficultySetting(ScaleSettingPreset.Preset3,
                GlobalSettings.Instance.ScaleSettingFlat,
                ref GlobalSettings.Instance.ScaleSettingFlat.Preset3,
                int.TryParse);


            for (int i = 10; i < 35; i += 5)
            {
                using (new GL.HorizontalScope())
                {
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingFlat.Flat(i, GlobalSettings.Instance.ScaleSettingFlat.Preset1)}", GL.Width(50));
                    GL.Space(50);
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingFlat.Flat(i, GlobalSettings.Instance.ScaleSettingFlat.Preset2)}", GL.Width(50));
                    GL.Space(60);
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingFlat.Flat(i, GlobalSettings.Instance.ScaleSettingFlat.Preset3)}", GL.Width(50));
                }
            }
        }
    }

    private static void OnGUIPercentBase()
    {
        using (new GL.VerticalScope())
        {
            GL.Label("Percent");
            GL.Label("This setting will add a percentage to the original DC.");
            DrawDifficultySetting(ScaleSettingPreset.Preset1,
                GlobalSettings.Instance.ScaleSettingPercent,
                ref GlobalSettings.Instance.ScaleSettingPercent.Preset1,
                float.TryParse);
            DrawDifficultySetting(ScaleSettingPreset.Preset2,
                GlobalSettings.Instance.ScaleSettingPercent,
                ref GlobalSettings.Instance.ScaleSettingPercent.Preset2,
                float.TryParse);
            DrawDifficultySetting(ScaleSettingPreset.Preset3,
                GlobalSettings.Instance.ScaleSettingPercent,
                ref GlobalSettings.Instance.ScaleSettingPercent.Preset3,
                float.TryParse);

            for (int i = 10; i < 35; i += 5)
            {
                using (new GL.HorizontalScope())
                {
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingPercent.Percentage(i, GlobalSettings.Instance.ScaleSettingPercent.Preset1)}", GL.Width(50));
                    GL.Space(50);
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingPercent.Percentage(i, GlobalSettings.Instance.ScaleSettingPercent.Preset2)}", GL.Width(50));
                    GL.Space(60);
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingPercent.Percentage(i, GlobalSettings.Instance.ScaleSettingPercent.Preset3)}", GL.Width(50));
                }
            }
        }
    }

    private static void OnGUIDiminishReturn()
    {


        using (new GL.VerticalScope())
        {
            GL.Label("Diminished Return f(x) = ((x + b) / (x * a) + c)");
            GL.Label("This setting will increase the original DC by a variable amount. The lower the DC the bigger the bonus");
            DrawDifficultySettingTuple(
                ScaleSettingPreset.Preset1,
                GlobalSettings.Instance.ScaleSettingDiminishingReturns,
                ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Preset1,
                float.TryParse);

            DrawDifficultySettingTuple(
                ScaleSettingPreset.Preset2,
                GlobalSettings.Instance.ScaleSettingDiminishingReturns,
                ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Preset2,
                float.TryParse);

            DrawDifficultySettingTuple(
                ScaleSettingPreset.Preset3,
                GlobalSettings.Instance.ScaleSettingDiminishingReturns,
                ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Preset3,
                float.TryParse);


            for (int i = 10; i < 35; i += 5)
            {
                using (new GL.HorizontalScope())
                {
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingDiminishingReturns.DiminishingReturns(i, GlobalSettings.Instance.ScaleSettingDiminishingReturns.Preset1)}", GL.Width(50));
                    GL.Space(50);
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingDiminishingReturns.DiminishingReturns(i, GlobalSettings.Instance.ScaleSettingDiminishingReturns.Preset2)}", GL.Width(50));
                    GL.Space(60);
                    GL.Label($"DC {i}->", GL.Width(55));
                    GL.Label($"{ScaleSettingDiminishingReturns.DiminishingReturns(i, GlobalSettings.Instance.ScaleSettingDiminishingReturns.Preset3)}", GL.Width(50));
                }
            }
        }
    }



    private static void DrawDCColumn<T>(int dcValue, Func<int, T, object> calculateDC, T preset)
    {
        GL.Label($"DC {dcValue}->", GL.Width(50));
        GL.Label($"{calculateDC(dcValue, preset)}", GL.Width(50));
    }

    private static void DrawDifficultySetting<T>(ScaleSettingPreset scaleSettingPreset, IScaleSetting presetObject, ref T presetValue, TryParseHandler<T> tryParse)
    {
        using (new GL.HorizontalScope())
        {
            if (GL.Toggle(scaleSettingPreset == presetObject.GetPreset(), Enum.GetName(typeof(ScaleSettingPreset), scaleSettingPreset), GL.Width(150)))
                presetObject.SetPreset(scaleSettingPreset);

            string input = GL.TextField(presetValue.ToString(), GL.Width(50));

            if (tryParse(input, out T result))
                presetValue = result;
            else
                GL.Label("Value must be a valid number");
        }
    }

    private static void DrawDifficultySettingTuple<T>(ScaleSettingPreset scaleSettingPreset, IScaleSetting presetObject, ref (T, T, T) presetValue, TryParseHandler<T> tryParse)
    {
        using (new GL.HorizontalScope())
        {
            if (GL.Toggle(scaleSettingPreset == presetObject.GetPreset(), Enum.GetName(typeof(ScaleSettingPreset), scaleSettingPreset), GL.Width(150)))
                presetObject.SetPreset(scaleSettingPreset);

            GL.Label("a: ", GL.Width(50));
            string input1 = GL.TextField(presetValue.Item1.ToString(), GL.Width(50));
            GL.Label("b: ", GL.Width(50));
            string input2 = GL.TextField(presetValue.Item2.ToString(), GL.Width(50));
            GL.Label("c: ", GL.Width(50));
            string input3 = GL.TextField(presetValue.Item3.ToString(), GL.Width(50));

            if (tryParse(input1, out T result1))
                presetValue.Item1 = result1;
            else
                GL.Label("Value must be a valid number");

            if (tryParse(input2, out T result2))
                presetValue.Item2 = result2;
            else
                GL.Label("Value must be a valid number");

            if (tryParse(input3, out T result3))
                presetValue.Item3 = result3;
            else
                GL.Label("Value must be a valid number");
        }
    }

#if DEBUG

    public static void OnGUIDebug(UnityModManager.ModEntry modEntry)
    {
        using (new GL.HorizontalScope())
        {
            if (GL.Button("Invoke Intializations", GL.Width(200)))
                Initializations.ExecuteInvokable();

            if (GL.Button("Dump Items", GL.Width(200)))
                FindAllItems.Dump();
        }

        OnGUI(modEntry);
    }
#endif
}
