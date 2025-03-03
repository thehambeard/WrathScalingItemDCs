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
        }

        if (_currentOnGUI == null)
            SetOnGui(GlobalSettings.Instance.CurrentSetting);

        _currentOnGUI?.Invoke();

        if(GL.Button("Save", GL.Width(200)))
            GlobalSettings.Instance.Save();
    }

    private delegate bool TryParseHandler<T>(string s, out T result);

    private static void OnGUIFlatBase()
    {
        GL.Label("Flat");

        using (new GL.VerticalScope())
        {
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
            DrawDifficultySetting("Normal Difficulty", ref GlobalSettings.Instance.ScaleSettingPercent.Normal, float.TryParse);
            DrawDifficultySetting("Hard Difficulty", ref GlobalSettings.Instance.ScaleSettingPercent.Hard, float.TryParse);
            DrawDifficultySetting("Unfair Difficulty", ref GlobalSettings.Instance.ScaleSettingPercent.Unfair, float.TryParse);
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

    private static void OnGUIStatBase()
    {
        GL.Label("Stat");
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
