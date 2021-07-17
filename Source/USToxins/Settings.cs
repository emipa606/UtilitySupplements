using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x02000015 RID: 21
    public class Settings : ModSettings
    {
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

        // Token: 0x0600003E RID: 62 RVA: 0x00003BD4 File Offset: 0x00001DD4
        public void DoWindowContents(Rect canvas)
        {
            var gap = 8f;
            var listing_Standard = new Listing_Standard {ColumnWidth = canvas.width};
            listing_Standard.Begin(canvas);
            listing_Standard.Gap();
            checked
            {
                listing_Standard.Label("USTox.ResPct".Translate() + "  " + (int) ResPct);
                ResPct = (int) listing_Standard.Slider((int) ResPct, 10f, 200f);
                listing_Standard.Gap(gap);
                Text.Font = GameFont.Tiny;
                listing_Standard.Label("          " + "USTox.ResWarn".Translate());
                listing_Standard.Gap(gap);
                listing_Standard.Label("          " + "USTox.ResTip".Translate());
                Text.Font = GameFont.Small;
                listing_Standard.Gap(gap);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("USTox.DoStinkrootGas".Translate(), ref doSRGas);
                listing_Standard.Gap(gap);
                listing_Standard.Label("USTox.SRGasRadius".Translate() + "  " + SRGasRadius);
                SRGasRadius = (int) listing_Standard.Slider(SRGasRadius, 1f, 3f);
                Text.Font = GameFont.Tiny;
                listing_Standard.Label("          " + "USTox.SRGasRadiusTip".Translate());
                Text.Font = GameFont.Small;
                listing_Standard.Gap(gap);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("USTox.DoTanglerootGas".Translate(), ref doTRGas);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("USTox.DoTanglerootCreep".Translate(), ref doTRCreep);
                listing_Standard.Gap(gap);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("USTox.DoAnimalFlee".Translate(), ref doAnimalFlee);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("USTox.DoAcidWalls".Translate(), ref doAdWalls);
                listing_Standard.Gap(gap);
                listing_Standard.Gap(gap);
                listing_Standard.Label("USTox.USToxLevels".Translate() + "  " + USToxLevels);
                USToxLevels = listing_Standard.Slider(USToxLevels, 50f, 200f);
                Text.Font = GameFont.Tiny;
                listing_Standard.Label("          " + "USTox.USToxLevelsTip".Translate());
                Text.Font = GameFont.Small;
                listing_Standard.Gap(gap);
                listing_Standard.End();
            }
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00003E98 File Offset: 0x00002098
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ResPct, "ResPct", 100f);
            Scribe_Values.Look(ref doSRGas, "doSRGas", true);
            Scribe_Values.Look(ref SRGasRadius, "SRGasRadius", 2);
            Scribe_Values.Look(ref doTRGas, "doTRGas", true);
            Scribe_Values.Look(ref doTRCreep, "doTRCreep", true);
            Scribe_Values.Look(ref doAnimalFlee, "doAnimalFlee", true);
            Scribe_Values.Look(ref doAdWalls, "doAdWalls", true);
            Scribe_Values.Look(ref USToxLevels, "USToxLevels", 100f);
        }
    }
}