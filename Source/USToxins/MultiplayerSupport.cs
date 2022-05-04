using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace USToxins;

[StaticConstructorOnStartup]
internal static class MultiplayerSupport
{
    private static readonly Harmony harmony = new Harmony("rimworld.pelador.utilitysupplements.multiplayersupport");

    static MultiplayerSupport()
    {
        if (!MP.enabled)
        {
            return;
        }

        MethodInfo[] array =
        {
            AccessTools.Method(typeof(Plant_TickLong), "GenRndVal"),
            AccessTools.Method(typeof(Plant_TickLong), "DoUSTangleCreepDmg"),
            AccessTools.Method(typeof(Plant_TickLong), "DoUSTangleDamage"),
            AccessTools.Method(typeof(USStinkrootGas), "DoUSToxicMental"),
            AccessTools.Method(typeof(USTanglerootGas), "DoUSTangleKillGasToxic"),
            AccessTools.Method(typeof(USTangleKillGas), "DoUSTangleKillGasToxic"),
            AccessTools.Method(typeof(USAntifreeze), "GetRndMelt"),
            AccessTools.Method(typeof(USAcidGas), "Rnd100"),
            AccessTools.Method(typeof(USAcidGas), "DoUSAcidGasToxic")
        };
        foreach (var methodInfo in array)
        {
            FixRNG(methodInfo);
        }
    }

    private static void FixRNG(MethodInfo method)
    {
        harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre"),
            new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos"));
    }

    private static void FixRNGPre()
    {
        Rand.PushState(Find.TickManager.TicksAbs);
    }

    private static void FixRNGPos()
    {
        Rand.PopState();
    }
}