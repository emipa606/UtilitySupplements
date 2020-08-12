using System;
using UnityEngine;
using Verse;

namespace USToxins
{
	// Token: 0x02000015 RID: 21
	public class Settings : ModSettings
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public void DoWindowContents(Rect canvas)
		{
			float gap = 8f;
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = canvas.width;
			listing_Standard.Begin(canvas);
			listing_Standard.Gap(12f);
			checked
			{
				listing_Standard.Label("USTox.ResPct".Translate() + "  " + (int)Settings.ResPct, -1f, null);
				Settings.ResPct = (float)((int)listing_Standard.Slider((float)((int)Settings.ResPct), 10f, 200f));
				listing_Standard.Gap(gap);
				Text.Font = GameFont.Tiny;
				listing_Standard.Label("          " + "USTox.ResWarn".Translate(), -1f, null);
				listing_Standard.Gap(gap);
				listing_Standard.Label("          " + "USTox.ResTip".Translate(), -1f, null);
				Text.Font = GameFont.Small;
				listing_Standard.Gap(gap);
				listing_Standard.Gap(gap);
				listing_Standard.CheckboxLabeled("USTox.DoStinkrootGas".Translate(), ref Settings.doSRGas, null);
				listing_Standard.Gap(gap);
				listing_Standard.Label("USTox.SRGasRadius".Translate() + "  " + Settings.SRGasRadius, -1f, null);
				Settings.SRGasRadius = (int)listing_Standard.Slider((float)Settings.SRGasRadius, 1f, 3f);
				Text.Font = GameFont.Tiny;
				listing_Standard.Label("          " + "USTox.SRGasRadiusTip".Translate(), -1f, null);
				Text.Font = GameFont.Small;
				listing_Standard.Gap(gap);
				listing_Standard.Gap(gap);
				listing_Standard.CheckboxLabeled("USTox.DoTanglerootGas".Translate(), ref Settings.doTRGas, null);
				listing_Standard.Gap(gap);
				listing_Standard.CheckboxLabeled("USTox.DoTanglerootCreep".Translate(), ref Settings.doTRCreep, null);
				listing_Standard.Gap(gap);
				listing_Standard.Gap(gap);
				listing_Standard.CheckboxLabeled("USTox.DoAnimalFlee".Translate(), ref Settings.doAnimalFlee, null);
				listing_Standard.Gap(gap);
				listing_Standard.CheckboxLabeled("USTox.DoAcidWalls".Translate(), ref Settings.doAdWalls, null);
				listing_Standard.Gap(gap);
				listing_Standard.Gap(gap);
				listing_Standard.Label("USTox.USToxLevels".Translate() + "  " + Settings.USToxLevels, -1f, null);
				Settings.USToxLevels = listing_Standard.Slider(Settings.USToxLevels, 50f, 200f);
				Text.Font = GameFont.Tiny;
				listing_Standard.Label("          " + "USTox.USToxLevelsTip".Translate(), -1f, null);
				Text.Font = GameFont.Small;
				listing_Standard.Gap(gap);
				listing_Standard.End();
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003E98 File Offset: 0x00002098
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref Settings.ResPct, "ResPct", 100f, false);
			Scribe_Values.Look<bool>(ref Settings.doSRGas, "doSRGas", true, false);
			Scribe_Values.Look<int>(ref Settings.SRGasRadius, "SRGasRadius", 2, false);
			Scribe_Values.Look<bool>(ref Settings.doTRGas, "doTRGas", true, false);
			Scribe_Values.Look<bool>(ref Settings.doTRCreep, "doTRCreep", true, false);
			Scribe_Values.Look<bool>(ref Settings.doAnimalFlee, "doAnimalFlee", true, false);
			Scribe_Values.Look<bool>(ref Settings.doAdWalls, "doAdWalls", true, false);
			Scribe_Values.Look<float>(ref Settings.USToxLevels, "USToxLevels", 100f, false);
		}

		// Token: 0x04000033 RID: 51
		public static float ResPct = 100f;

		// Token: 0x04000034 RID: 52
		public static bool doSRGas = true;

		// Token: 0x04000035 RID: 53
		public static int SRGasRadius = 2;

		// Token: 0x04000036 RID: 54
		public static bool doTRGas = true;

		// Token: 0x04000037 RID: 55
		public static bool doTRCreep = true;

		// Token: 0x04000038 RID: 56
		public static bool doAnimalFlee = true;

		// Token: 0x04000039 RID: 57
		public static bool doAdWalls = true;

		// Token: 0x0400003A RID: 58
		public static float USToxLevels = 100f;
	}
}
