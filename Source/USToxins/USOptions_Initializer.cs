using System;
using System.Collections.Generic;
using Verse;

namespace USToxins
{
	// Token: 0x02000021 RID: 33
	[StaticConstructorOnStartup]
	internal static class USOptions_Initializer
	{
		// Token: 0x06000075 RID: 117 RVA: 0x00005280 File Offset: 0x00003480
		static USOptions_Initializer()
		{
			LongEventHandler.QueueLongEvent(new Action(USOptions_Initializer.Setup), "LibraryStartup", false, null, true);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000529C File Offset: 0x0000349C
		public static void Setup()
		{
			List<ResearchProjectDef> allDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
			if (allDefs.Count > 0)
			{
				List<string> USList = USOptions_Initializer.USResearchList();
				foreach (ResearchProjectDef ResDef in allDefs)
				{
					if (USList.Contains(ResDef.defName))
					{
						float Resbase = ResDef.baseCost;
						Resbase = (float)(checked((int)Math.Round((double)(unchecked(Resbase * (Settings.ResPct / 100f))))));
						ResDef.baseCost = Resbase;
					}
				}
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005330 File Offset: 0x00003530
		public static List<string> USResearchList()
		{
			List<string> list = new List<string>();
			list.AddDistinct("USStinkroot");
			list.AddDistinct("USTangleroot");
			list.AddDistinct("USFilthKill");
			list.AddDistinct("USAntifreeze");
			list.AddDistinct("USInsectKill");
			list.AddDistinct("USMindKill");
			list.AddDistinct("USGlowers");
			list.AddDistinct("USPlantKill");
			list.AddDistinct("USTreeKill");
			list.AddDistinct("USWeedKill");
			list.AddDistinct("USBlightKill");
			list.AddDistinct("USTangleKill");
			list.AddDistinct("USTearGas");
			list.AddDistinct("USAmmoniaFert");
			list.AddDistinct("USAcidGas");
			return list;
		}
	}
}
