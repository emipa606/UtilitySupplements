using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins
{
	// Token: 0x02000016 RID: 22
	[HarmonyPatch(typeof(FilthMaker), "TerrainAcceptsFilth")]
	public class TerrainAcceptsFilthPatch
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00003F7D File Offset: 0x0000217D
		[HarmonyPostfix]
		public static void Postfix(ref bool __result, TerrainDef terrainDef, ThingDef filthDef, FilthSourceFlags additionalFlags = FilthSourceFlags.None)
		{
			if (!__result && terrainDef.AffectsFertility && Globals.IsUSFilth(filthDef))
			{
				__result = true;
			}
		}
	}
}
