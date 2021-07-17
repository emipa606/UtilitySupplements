using Verse;

namespace USToxins
{
    // Token: 0x0200000F RID: 15
    internal class Globals
    {
        // Token: 0x04000009 RID: 9
        internal const int DryOutTime = 60000;

        // Token: 0x0400000A RID: 10
        internal const int DamTickPeriod = 120;

        // Token: 0x0400000B RID: 11
        internal const int PlantToxicDmg = 2;

        // Token: 0x0400000C RID: 12
        internal const int TreeToxicDmg = 4;

        // Token: 0x0400000D RID: 13
        internal const int WeedToxicDmg = 2;

        // Token: 0x0400000E RID: 14
        internal const float CleanDmgResist = 0.5f;

        // Token: 0x0400000F RID: 15
        internal const int InsectBurnDmg = 3;

        // Token: 0x04000010 RID: 16
        internal const float InsectHeatSev = 0.02f;

        // Token: 0x04000011 RID: 17
        internal const float BlightToxicDmg = 0.1f;

        // Token: 0x04000012 RID: 18
        internal const float StinkRootGasSev = 0.05f;

        // Token: 0x04000013 RID: 19
        internal const float StinkRootMindBreakPt = 2.5f;

        // Token: 0x04000014 RID: 20
        internal const float StinkRootMindChance = 0.95f;

        // Token: 0x04000015 RID: 21
        internal const float MindKillFactor = 5f;

        // Token: 0x04000016 RID: 22
        internal const float TangleCutDmg = 10f;

        // Token: 0x04000017 RID: 23
        internal const float TangleStunDmg = 30f;

        // Token: 0x04000018 RID: 24
        internal const float TangleRootSev = 0.05f;

        // Token: 0x04000019 RID: 25
        internal const float TangleRootFactor = 1f;

        // Token: 0x0400001A RID: 26
        internal const float TangleKillFactor = 2.5f;

        // Token: 0x0400001B RID: 27
        internal const float TangleRootMaxSev = 1.8f;

        // Token: 0x0400001C RID: 28
        internal const float TanglerootCreepChance = 0.03f;

        // Token: 0x0400001D RID: 29
        internal const float TearGasSev = 0.125f;

        // Token: 0x0400001E RID: 30
        internal const float AcidGasFactor = 1f;

        // Token: 0x0400001F RID: 31
        internal const float AcidGasSev = 0.1f;

        // Token: 0x04000020 RID: 32
        internal const string USStinkyPlantdefName = "Plant_USStinkroot";

        // Token: 0x04000021 RID: 33
        internal const string USTanglePlantdefName = "Plant_USTangleroot";

        // Token: 0x04000022 RID: 34
        internal static DamageDef USPlantToxin = DefDatabase<DamageDef>.GetNamed("Dam_USPlantToxin");

        // Token: 0x04000023 RID: 35
        internal static HediffDef USStinkRootGas = DefDatabase<HediffDef>.GetNamed("HED_USStinkRootGas");

        // Token: 0x04000024 RID: 36
        internal static ThingDef USStinkyGas = DefDatabase<ThingDef>.GetNamed("Gas_USStinkroot");

        // Token: 0x04000025 RID: 37
        internal static HediffDef USTangleRootStrike = DefDatabase<HediffDef>.GetNamed("HED_USTangleRoot");

        // Token: 0x04000026 RID: 38
        internal static ThingDef USTangleGas = DefDatabase<ThingDef>.GetNamed("Gas_USTangleroot");

        // Token: 0x04000027 RID: 39
        internal static ThingDef USTanglePlant = DefDatabase<ThingDef>.GetNamed("Plant_USTangleroot");

        // Token: 0x04000028 RID: 40
        internal static float USTanglePlantFertMin = USTanglePlant.plant.fertilityMin;

        // Token: 0x04000029 RID: 41
        internal static HediffDef USTearGas = DefDatabase<HediffDef>.GetNamed("HED_USTearGas");

        // Token: 0x0400002A RID: 42
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

        // Token: 0x06000026 RID: 38 RVA: 0x00002EC4 File Offset: 0x000010C4
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

                        if (defName == "Filth_USAmmoniaFertFoam")
                        {
                            return true;
                        }
                    }
                    else if (defName == "Filth_USWeedKillFoam")
                    {
                        return true;
                    }
                }
                else if (num != 1527094750U)
                {
                    if (num != 1809654034U)
                    {
                        return false;
                    }

                    if (defName == "Filth_USBlightKillFoam")
                    {
                        return true;
                    }
                }
                else if (defName == "Filth_USAntifreeze")
                {
                    return true;
                }
            }
            else if (num <= 1922711255U)
            {
                if (num != 1887853120U)
                {
                    if (num != 1922711255U)
                    {
                        return false;
                    }

                    if (defName == "Filth_USPlantKillFoam")
                    {
                        return true;
                    }
                }
                else if (defName == "Filth_USInsectKillFoam")
                {
                    return true;
                }
            }
            else if (num != 3589335437U)
            {
                if (num != 3965722172U)
                {
                    if (num != 4098764337U)
                    {
                        return false;
                    }

                    if (defName == "Filth_USFilthKillFoam")
                    {
                        return true;
                    }
                }
                else if (defName == "Filth_USTreeKillFoam")
                {
                    return true;
                }
            }
            else if (defName == "Filth_USGlowFoam")
            {
                return true;
            }

            return false;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002FFC File Offset: 0x000011FC
        internal static bool USVictimImmuneTo(Pawn pawn, HediffDef def)
        {
            var hediffs = pawn.health.hediffSet.hediffs;
            foreach (var hediff in hediffs)
            {
                var curStage = hediff.CurStage;
                if (curStage == null || curStage.makeImmuneTo == null)
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
}