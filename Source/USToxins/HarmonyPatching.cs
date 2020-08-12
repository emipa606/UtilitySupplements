using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace USToxins
{
	// Token: 0x0200001E RID: 30
	[StaticConstructorOnStartup]
	internal static class HarmonyPatching
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00004C79 File Offset: 0x00002E79
		static HarmonyPatching()
		{
			new Harmony("com.Pelador.RW.US.USToxins").PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
