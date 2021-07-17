using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace USToxins
{
    // Token: 0x02000012 RID: 18
    [StaticConstructorOnStartup]
    internal static class MultiplayerSupport
    {
        // Token: 0x04000031 RID: 49
        private static readonly Harmony harmony = new Harmony("rimworld.pelador.utilitysupplements.multiplayersupport");

        // Token: 0x06000032 RID: 50 RVA: 0x00003420 File Offset: 0x00001620
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

        // Token: 0x06000033 RID: 51 RVA: 0x00003543 File Offset: 0x00001743
        private static void FixRNG(MethodInfo method)
        {
            harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre"),
                new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos"));
        }

        // Token: 0x06000034 RID: 52 RVA: 0x0000357D File Offset: 0x0000177D
        private static void FixRNGPre()
        {
            Rand.PushState(Find.TickManager.TicksAbs);
        }

        // Token: 0x06000035 RID: 53 RVA: 0x0000358E File Offset: 0x0000178E
        private static void FixRNGPos()
        {
            Rand.PopState();
        }
    }
}