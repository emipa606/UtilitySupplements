using System;
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
		// Token: 0x06000032 RID: 50 RVA: 0x00003420 File Offset: 0x00001620
		static MultiplayerSupport()
		{
			if (!MP.enabled)
			{
				return;
			}
			MethodInfo[] array = new MethodInfo[]
			{
				AccessTools.Method(typeof(Plant_TickLong), "GenRndVal", null, null),
				AccessTools.Method(typeof(Plant_TickLong), "DoUSTangleCreepDmg", null, null),
				AccessTools.Method(typeof(Plant_TickLong), "DoUSTangleDamage", null, null),
				AccessTools.Method(typeof(USStinkrootGas), "DoUSToxicMental", null, null),
				AccessTools.Method(typeof(USTanglerootGas), "DoUSTangleKillGasToxic", null, null),
				AccessTools.Method(typeof(USTangleKillGas), "DoUSTangleKillGasToxic", null, null),
				AccessTools.Method(typeof(USAntifreeze), "GetRndMelt", null, null),
				AccessTools.Method(typeof(USAcidGas), "Rnd100", null, null),
				AccessTools.Method(typeof(USAcidGas), "DoUSAcidGasToxic", null, null)
			};
			for (int i = 0; i < array.Length; i++)
			{
				MultiplayerSupport.FixRNG(array[i]);
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003543 File Offset: 0x00001743
		private static void FixRNG(MethodInfo method)
		{
			MultiplayerSupport.harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre", null), new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos", null), null, null);
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

		// Token: 0x04000031 RID: 49
		private static Harmony harmony = new Harmony("rimworld.pelador.utilitysupplements.multiplayersupport");
	}
}
