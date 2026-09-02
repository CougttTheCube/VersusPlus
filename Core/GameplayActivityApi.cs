#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
namespace VersusPlus;

using HarmonyLib;

using Il2CppReloaded.TreeStateActivities;

[HarmonyPatch]
internal static class GameplayActivityApi
{
    public static event Action<GameplayActivity>? ActiveStarted;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.ActiveStarted))]
    private static void OnActiveStarted(GameplayActivity __instance)
    {
        ActiveStarted?.Invoke(__instance);
    }
}
