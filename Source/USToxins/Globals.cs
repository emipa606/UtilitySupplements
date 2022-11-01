using Verse;

namespace USToxins;

internal class Globals
{
    internal const int DryOutTime = 60000;

    internal const int DamTickPeriod = 120;

    internal const int PlantToxicDmg = 2;

    internal const int TreeToxicDmg = 4;

    internal const int WeedToxicDmg = 2;

    internal const float CleanDmgResist = 0.5f;

    internal const int InsectBurnDmg = 3;

    internal const float InsectHeatSev = 0.02f;

    internal const float BlightToxicDmg = 0.1f;

    internal const float StinkRootGasSev = 0.05f;

    internal const float StinkRootMindBreakPt = 2.5f;

    internal const float StinkRootMindChance = 0.95f;

    internal const float MindKillFactor = 5f;

    internal const float TangleCutDmg = 10f;

    internal const float TangleStunDmg = 30f;

    internal const float TangleRootSev = 0.05f;

    internal const float TangleRootFactor = 1f;

    internal const float TangleKillFactor = 2.5f;

    internal const float TangleRootMaxSev = 1.8f;

    internal const float TanglerootCreepChance = 0.03f;

    internal const float TearGasSev = 0.125f;

    internal const float AcidGasFactor = 1f;

    internal const float AcidGasSev = 0.1f;

    internal const string USStinkyPlantdefName = "Plant_USStinkroot";

    internal const string USTanglePlantdefName = "Plant_USTangleroot";

    internal static DamageDef USPlantToxin = DefDatabase<DamageDef>.GetNamed("Dam_USPlantToxin");

    internal static HediffDef USStinkRootGas = DefDatabase<HediffDef>.GetNamed("HED_USStinkRootGas");

    internal static ThingDef USStinkyGas = DefDatabase<ThingDef>.GetNamed("Gas_USStinkroot");

    internal static HediffDef USTangleRootStrike = DefDatabase<HediffDef>.GetNamed("HED_USTangleRoot");

    internal static ThingDef USTangleGas = DefDatabase<ThingDef>.GetNamed("Gas_USTangleroot");

    internal static ThingDef USTanglePlant = DefDatabase<ThingDef>.GetNamed("Plant_USTangleroot");

    internal static float USTanglePlantFertMin = USTanglePlant.plant.fertilityMin;

    internal static HediffDef USTearGas = DefDatabase<HediffDef>.GetNamed("HED_USTearGas");

    internal static HediffDef USAcidGasEffect = DefDatabase<HediffDef>.GetNamed("HED_USAcidGas");

    internal static uint ComputeStringHash(string s)
    {
        uint num = 0;
        if (s == null)
        {
            return num;
        }

        num = 2166136261U;
        foreach (var c in s)
        {
            num = (c ^ num) * 16777619U;
        }

        return num;
    }

    internal static bool IsUSFilth(ThingDef filthdef)
    {
        var defName = filthdef.defName;
        var num = ComputeStringHash(defName);
        if (num <= 1809654034U)
        {
            if (num <= 1372496259U)
            {
                if (num != 1311158745U)
                {
                    if (num != 1372496259U)
                    {
                        return false;
                    }

                    return defName == "Filth_USAmmoniaFertFoam";
                }

                return defName == "Filth_USWeedKillFoam";
            }

            if (num != 1527094750U)
            {
                if (num != 1809654034U)
                {
                    return false;
                }

                return defName == "Filth_USBlightKillFoam";
            }

            return defName == "Filth_USAntifreeze";
        }

        if (num <= 1922711255U)
        {
            if (num != 1887853120U)
            {
                if (num != 1922711255U)
                {
                    return false;
                }

                return defName == "Filth_USPlantKillFoam";
            }

            return defName == "Filth_USInsectKillFoam";
        }

        if (num != 3589335437U)
        {
            if (num != 3965722172U)
            {
                if (num != 4098764337U)
                {
                    return false;
                }

                return defName == "Filth_USFilthKillFoam";
            }

            return defName == "Filth_USTreeKillFoam";
        }

        return defName == "Filth_USGlowFoam";
    }

    internal static bool USVictimImmuneTo(Pawn pawn, HediffDef def)
    {
        var hediffs = pawn.health.hediffSet.hediffs;
        foreach (var hediff in hediffs)
        {
            var curStage = hediff.CurStage;
            if (curStage?.makeImmuneTo == null)
            {
                continue;
            }

            foreach (var hediffDef in curStage.makeImmuneTo)
            {
                if (hediffDef == def)
                {
                    return true;
                }
            }
        }

        return false;
    }
}