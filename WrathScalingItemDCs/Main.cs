using System.Reflection;
using UnityModManagerNet;
using WrathHamCore.Utility.Initialization;
using WrathScalingItemDCs.UI_UMM;

namespace WrathScalingItemDCs;

#if DEBUG
[EnableReloading]
#endif
public static class Main
{
    internal static UnityModManager.ModEntry ModEntry { get; private set; }
    internal static WrathHamCore.Utility.Logger Logger { get; private set; }
    internal static WrathHamCore.Utility.ModEventHandler ModEventHandler { get; private set; }

    public static bool Load(UnityModManager.ModEntry modEntry)
    {
        ModEntry = modEntry;
        Logger = new(modEntry.Logger);

        WrathHamCore.ModReference.Initialize(Logger, modEntry);

        ModEventHandler = new();
        modEntry.OnToggle = OnToggle;

        Initializations.ExecuteOnLaunch();
#if DEBUG
        modEntry.OnUnload = OnUnload;
        modEntry.OnGUI = UMMMenu.OnGUIDebug;
#else
        modEntry.OnGUI = UMMMenu.OnGUI;
#endif
        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
    {
        if (value)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            ModEventHandler.Enable(modEntry, assembly);
        }
        else
        {
            ModEventHandler.Disable(modEntry);
        }

        return true;
    }

#if DEBUG
    public static bool OnUnload(UnityModManager.ModEntry modEntry)
    {
        return true;
    }
#endif
}
