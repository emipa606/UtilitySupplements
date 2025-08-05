using System;
using System.Collections.Generic;
using Verse;

namespace USToxins;

[StaticConstructorOnStartup]
internal static class USOptions_Initializer
{
    static USOptions_Initializer()
    {
        LongEventHandler.QueueLongEvent(Setup, "LibraryStartup", false, null);
    }

    private static void Setup()
    {
        var allDefs = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
        if (allDefs.Count <= 0)
        {
            return;
        }

        var USList = USResearchList();
        foreach (var ResDef in allDefs)
        {
            if (!USList.Contains(ResDef.defName))
            {
                continue;
            }

            var Resbase = ResDef.baseCost;
            Resbase = checked((int)Math.Round(Resbase * (Settings.ResPct / 100f)));
            ResDef.baseCost = Resbase;
        }
    }

    private static List<string> USResearchList()
    {
        var list = new List<string>();
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