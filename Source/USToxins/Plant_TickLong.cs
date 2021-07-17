using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000014 RID: 20
    [HarmonyPatch(typeof(Plant), "TickLong")]
    public class Plant_TickLong
    {
        // Token: 0x04000032 RID: 50
        public static float toxRatio = Settings.USToxLevels / 100f;

        // Token: 0x06000038 RID: 56 RVA: 0x00003610 File Offset: 0x00001810
        [HarmonyPostfix]
        public static void Postfix(ref Plant __instance)
        {
            if (__instance == null)
            {
                return;
            }

            if (__instance.DestroyedOrNull())
            {
                return;
            }

            if (__instance.def.defName != "Plant_USStinkroot" && __instance.def.defName != "Plant_USTangleroot")
            {
                return;
            }

            var TargetMap = __instance.Map;
            var TargetCell = __instance.Position;
            var Plantlist = TargetCell.GetThingList(TargetMap);
            if (Plantlist.Count <= 0)
            {
                return;
            }

            foreach (var thing in Plantlist)
            {
                if (thing == null || thing is not Plant plant)
                {
                    continue;
                }

                if (plant.def.defName == "Plant_USStinkroot" && Settings.doSRGas)
                {
                    if (plant.Position != TargetCell)
                    {
                        continue;
                    }

                    var stinkyRadius = Settings.SRGasRadius;
                    var maturity = plant.Growth;
                    stinkyRadius = (int) (stinkyRadius * maturity);
                    if (stinkyRadius < 1)
                    {
                        stinkyRadius = 1;
                    }

                    var stinkycells = GenRadial.RadialCellsAround(TargetCell, stinkyRadius, true).ToList();
                    if (stinkycells.Count <= 0)
                    {
                        continue;
                    }

                    foreach (var chkcell in stinkycells)
                    {
                        if (chkcell.IsValid && chkcell.InBounds(TargetMap) &&
                            !chkcell.Impassable(TargetMap))
                        {
                            GenSpawn.Spawn(Globals.USStinkyGas, chkcell, TargetMap);
                        }
                    }
                }
                else if (plant.def.defName == "Plant_USTangleroot" &&
                         plant.Position == TargetCell)
                {
                    var Pawnlist = TargetCell.GetThingList(TargetMap);
                    if (Pawnlist.Count > 0)
                    {
                        foreach (var thing1 in Pawnlist)
                        {
                            if (thing1 is Pawn pawn && pawn.Position == TargetCell)
                            {
                                DoUSTangleDamage(pawn, plant);
                            }
                        }
                    }

                    if (Settings.doTRGas && TargetCell.IsValid && TargetCell.InBounds(TargetMap) &&
                        !TargetCell.Impassable(TargetMap))
                    {
                        GenSpawn.Spawn(Globals.USTangleGas, TargetCell, TargetMap);
                    }

                    if (!Settings.doTRCreep || !(__instance.Growth >= 1f) || !(GenRndVal(0, 1) <= 0.03f))
                    {
                        continue;
                    }

                    var chkcreepcells = GenRadial.RadialCellsAround(TargetCell, 1f, false).ToList();
                    if (chkcreepcells.Count <= 0)
                    {
                        continue;
                    }

                    var creepcelllist = new List<IntVec3>();
                    creepcelllist.Clear();
                    foreach (var chkcreepcell in chkcreepcells)
                    {
                        if (!chkcreepcell.IsValid || !chkcreepcell.InBounds(TargetMap) ||
                            chkcreepcell.Impassable(TargetMap) || !(chkcreepcell.GetTerrain(TargetMap).fertility >=
                                                                    Globals.USTanglePlantFertMin))
                        {
                            continue;
                        }

                        var AddToList = true;
                        var SamePlantlist = chkcreepcell.GetThingList(TargetMap);
                        if (SamePlantlist.Count > 0)
                        {
                            foreach (var thing1 in SamePlantlist)
                            {
                                if (thing1 is Fire)
                                {
                                    AddToList = false;
                                }

                                if (thing1 is Plant && thing1.def.defName ==
                                    "Plant_USTangleroot")
                                {
                                    AddToList = false;
                                }
                            }
                        }

                        if (AddToList)
                        {
                            creepcelllist.Add(chkcreepcell);
                        }
                    }

                    if (creepcelllist.Count <= 0)
                    {
                        continue;
                    }

                    var rndcreeper = GenRndVal(0, creepcelllist.Count);
                    rndcreeper--;
                    if (rndcreeper < 0)
                    {
                        rndcreeper = 0;
                    }

                    var creeperCell = creepcelllist[rndcreeper];
                    if (!creeperCell.IsValid || creeperCell == TargetCell)
                    {
                        continue;
                    }

                    var ClearPlantlist = creeperCell.GetThingList(TargetMap);
                    if (ClearPlantlist.Count > 0)
                    {
                        foreach (var thing1 in ClearPlantlist)
                        {
                            if (thing1 is not Plant plant1 || plant1.def.defName == "Plant_USTangleroot")
                            {
                                continue;
                            }

                            var kill = !plant1.def.plant.IsTree;

                            DoUSTangleCreepDmg(plant1, kill, __instance);
                        }
                    }

                    var plantnew = GenSpawn.Spawn(Globals.USTanglePlant, creeperCell,
                        TargetMap);
                    if (plantnew.DestroyedOrNull() || plantnew is not Plant plantnew1)
                    {
                        continue;
                    }

                    plantnew1.Growth = 0.05f;
                    TargetMap.mapDrawer.MapMeshDirty(plantnew1.Position, MapMeshFlag.Things);
                }
            }
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00003AC0 File Offset: 0x00001CC0
        public static int GenRndVal(int first, int second)
        {
            return Rand.Range(first, second);
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00003ACC File Offset: 0x00001CCC
        public static void DoUSTangleCreepDmg(Thing T, bool kill, Thing C)
        {
            if (T is not Plant)
            {
                return;
            }

            if (kill)
            {
                T.Destroy();
                return;
            }

            var Dmg = Rand.Range(1f, 51f);
            if (Dmg < 1f)
            {
                Dmg = 1f;
            }

            T.TakeDamage(new DamageInfo(DamageDefOf.Cut, Dmg, 0f, -1f, C));
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00003B2C File Offset: 0x00001D2C
        public static void DoUSTangleDamage(Pawn P, Thing T)
        {
            if (P == null)
            {
                return;
            }

            var Dmg = Rand.Range(1f, 11f);
            if (Dmg < 1f)
            {
                Dmg = 1f;
            }

            P.TakeDamage(new DamageInfo(DamageDefOf.Cut, Dmg, 0f, -1f, T));
            var StunDmg = Rand.Range(1f, 31f);
            if (StunDmg < 1f)
            {
                StunDmg = 1f;
            }

            P.TakeDamage(new DamageInfo(DamageDefOf.Stun, StunDmg, 0f, -1f, T));
        }
    }
}