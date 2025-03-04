using Kingmaker.UI.MVVM._ConsoleView.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        if (GL.Button("Save", GL.Width(200)))
            GlobalSettings.Instance.Save();
    }

    private delegate bool TryParseHandler<T>(string s, out T result);

    private static void OnGUIFlatBase()
    {
        GL.Label("Flat");

        using (new GL.VerticalScope())
        {
            DrawDifficultyToggle(ref GlobalSettings.Instance.ScaleSettingFlat.Difficulty);
            DrawDifficultySetting("Normal Difficulty", ref GlobalSettings.Instance.ScaleSettingFlat.Normal, int.TryParse);
            DrawDifficultySetting("Hard Difficulty", ref GlobalSettings.Instance.ScaleSettingFlat.Hard, int.TryParse);
            DrawDifficultySetting("Unfair Difficulty", ref GlobalSettings.Instance.ScaleSettingFlat.Unfair, int.TryParse);
        }
    }

    private static void OnGUIPercentBase()
    {
        GL.Label("Percent");

        using (new GL.VerticalScope())
        {
            DrawDifficultyToggle(ref GlobalSettings.Instance.ScaleSettingPercent.Difficulty);
            DrawDifficultySetting("Normal Difficulty", ref GlobalSettings.Instance.ScaleSettingPercent.Normal, float.TryParse);
            DrawDifficultySetting("Hard Difficulty", ref GlobalSettings.Instance.ScaleSettingPercent.Hard, float.TryParse);
            DrawDifficultySetting("Unfair Difficulty", ref GlobalSettings.Instance.ScaleSettingPercent.Unfair, float.TryParse);
        }
    }

    private static void OnGUIDiminishReturn()
    {
        GL.Label("Diminished Return f(x) = (x + b) / (x * a) + c)");

        using (new GL.VerticalScope())
        {
            DrawDifficultyToggle(ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Difficulty);
            DrawDifficultySettingTuple("Normal Difficulty", ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Normal, float.TryParse);
            DrawDifficultySettingTuple("Hard Difficulty", ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Hard, float.TryParse);
            DrawDifficultySettingTuple("Unfair Difficulty", ref GlobalSettings.Instance.ScaleSettingDiminishingReturns.Unfair, float.TryParse);

            for (int i = 10; i < 35; i += 5)
            {
                using (new GL.HorizontalScope())
                {
                    GL.Label($"DC {i}:", GL.Width(50));
                    GL.Label($"{ScaleSettingDiminishingReturns.DiminishingReturns(i, GlobalSettings.Instance.ScaleSettingDiminishingReturns.Normal)}", GL.Width(50));
                    GL.Space(50);
                    GL.Label($"DC {i}:", GL.Width(50));
                    GL.Label($"{ScaleSettingDiminishingReturns.DiminishingReturns(i, GlobalSettings.Instance.ScaleSettingDiminishingReturns.Hard)}", GL.Width(50));
                    GL.Space(60);
                    GL.Label($"DC {i}:", GL.Width(50));
                    GL.Label($"{ScaleSettingDiminishingReturns.DiminishingReturns(i, GlobalSettings.Instance.ScaleSettingDiminishingReturns.Unfair)}", GL.Width(50));
                }
            }
        }
    }

    private static void DrawDifficultyToggle(ref ScaleSettingDifficulty scaleSettingDifficulty)
    {
        using (new GL.HorizontalScope())
        {
            foreach (ScaleSettingDifficulty difficulty in Enum.GetValues(typeof(ScaleSettingDifficulty)))
            {
                bool isSelected = scaleSettingDifficulty == difficulty;
                if (GL.Toggle(isSelected, Enum.GetName(typeof(ScaleSettingDifficulty), difficulty), GL.Width(150)))
                    scaleSettingDifficulty = difficulty;
            }
        }
    }


    private static void DrawDifficultySetting<T>(string label, ref T difficultyValue, TryParseHandler<T> tryParse)
    {
        using (new GL.HorizontalScope())
        {
            GL.Label(label, GL.Width(150));
            string input = GL.TextField(difficultyValue.ToString(), GL.Width(50));

            if (tryParse(input, out T result))
                difficultyValue = result;
            else
                GL.Label("Value must be a valid number");
        }
    }

    private static void DrawDifficultySettingTuple<T>(string label, ref (T, T, T) difficultyValue, TryParseHandler<T> tryParse)
    {
        using (new GL.HorizontalScope())
        {
            GL.Label(label, GL.Width(200));
            GL.Label("a: ", GL.Width(50));
            string input1 = GL.TextField(difficultyValue.Item1.ToString(), GL.Width(50));
            GL.Label("b: ", GL.Width(50));
            string input2 = GL.TextField(difficultyValue.Item2.ToString(), GL.Width(50));
            GL.Label("c: ", GL.Width(50));
            string input3 = GL.TextField(difficultyValue.Item3.ToString(), GL.Width(50));

            if (tryParse(input1, out T result1))
                difficultyValue.Item1 = result1;
            else
                GL.Label("Value must be a valid number");

            if (tryParse(input2, out T result2))
                difficultyValue.Item2 = result2;
            else
                GL.Label("Value must be a valid number");

            if (tryParse(input3, out T result3))
                difficultyValue.Item3 = result3;
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
