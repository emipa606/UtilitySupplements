using UnityEngine;
using Verse;

namespace USToxins;

public class Settings : ModSettings
{
    public static float ResPct = 100f;

    public static bool doSRGas = true;

    public static int SRGasRadius = 2;

    public static bool doTRGas = true;

    public static bool doTRCreep = true;

    public static bool doAnimalFlee = true;

    public static bool doAdWalls = true;

    public static float USToxLevels = 100f;

    public static void DoWindowContents(Rect canvas)
    {
        var gap = 8f;
        var listing_Standard = new Listing_Standard { ColumnWidth = canvas.width };
        listing_Standard.Begin(canvas);
        listing_Standard.Gap();
        checked
        {
            listing_Standard.Label("USTox.ResPct".Translate() + "  " + (int)ResPct);
            ResPct = (int)listing_Standard.Slider((int)ResPct, 10f, 200f);
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
            SRGasRadius = (int)listing_Standard.Slider(SRGasRadius, 1f, 3f);
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
            if (Controller.currentVersion != null)
            {
                listing_Standard.Gap();
                GUI.contentColor = Color.gray;
                listing_Standard.Label("USTox.CurrentModVersion".Translate(Controller.currentVersion));
                GUI.contentColor = Color.white;
            }

            listing_Standard.End();
        }
    }

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