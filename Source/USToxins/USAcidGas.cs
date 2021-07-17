using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace USToxins
{
    // Token: 0x02000018 RID: 24
    public class USAcidGas : Gas
    {
        // Token: 0x0400003C RID: 60
        public static DamageDef USAcid = DefDatabase<DamageDef>.GetNamed("Dam_USAcidGas");

        // Token: 0x0400003B RID: 59
        public int amountAcidDam = 1000;

        // Token: 0x0400003D RID: 61
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x06000046 RID: 70 RVA: 0x00003FDB File Offset: 0x000021DB
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, true);
            destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00004010 File Offset: 0x00002210
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref destroyTick, "destroyTick");
        }

        // Token: 0x06000048 RID: 72 RVA: 0x0000402C File Offset: 0x0000222C
        public override void Tick()
        {
            if (destroyTick <= Find.TickManager.TicksGame || amountAcidDam <= 0)
            {
                Destroy();
            }

            graphicRotation += graphicRotationSpeed;
            if (Destroyed || Find.TickManager.TicksGame % 120 != 0)
            {
                return;
            }

            var TargetMap = Map;
            var TargetCell = Position;
            var Thinglist = TargetCell.GetThingList(TargetMap);
            if (Thinglist.Count > 0)
            {
                foreach (var thing in Thinglist)
                {
                    if (thing is Gas || thing.Position != TargetCell)
                    {
                        continue;
                    }

                    if (thing.def.useHitPoints && !(thing is Pawn) ||
                        thing.def.useHitPoints && thing.def.IsCorpse)
                    {
                        DoUSAcidGasDegrade(this, thing, out var DMG);
                        amountAcidDam -= DMG;
                    }
                    else if (thing is Pawn pawn && !pawn.def.IsCorpse)
                    {
                        DoUSAcidGasPawn(this, pawn);
                        if (PawnIsAlive(pawn) &&
                            !pawn.RaceProps.IsMechanoid &&
                            pawn.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
                        {
                            DoUSAcidGasToxic(this, pawn);
                        }
                    }
                }
            }

            if (!Settings.doAdWalls)
            {
                return;
            }

            var cells = GenAdjFast.AdjacentCellsCardinal(TargetCell);
            if (cells.Count <= 0)
            {
                return;
            }

            foreach (var cell in cells)
            {
                if (!cell.InBounds(TargetMap))
                {
                    continue;
                }

                var edifice = cell.GetEdifice(TargetMap);
                if (edifice == null)
                {
                    continue;
                }

                DoUSAcidGasDegrade(this, edifice, out var DMG2);
                amountAcidDam -= DMG2;
            }
        }

        // Token: 0x06000049 RID: 73 RVA: 0x0000426C File Offset: 0x0000246C
        public static bool PawnIsAlive(Pawn pawn)
        {
            return pawn is {Spawned: true, Dead: false} && !pawn.def.IsCorpse;
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00004294 File Offset: 0x00002494
        public void DoUSAcidGasDegrade(Thing Gas, Thing targ, out int DMG)
        {
            var baseDMG = Math.Max(1f, Math.Min(10f, GetBaseDMG(targ)));
            DMG = (int) baseDMG;
            targ.HitPoints -= DMG;
            if (targ.HitPoints <= 0 && !targ.Destroyed)
            {
                targ.Destroy();
            }
        }

        // Token: 0x0600004B RID: 75 RVA: 0x000042E8 File Offset: 0x000024E8
        public float GetBaseDMG(Thing targ)
        {
            if (targ is Building || targ.def.IsMetal || targ.def.IsShell)
            {
                return 6f;
            }

            if (targ.def.IsWeapon)
            {
                return 5f;
            }

            if (targ is Apparel || targ.def.IsLeather)
            {
                return 3f;
            }

            if (targ.def.IsDrug || targ.def.IsMedicine)
            {
                return 4f;
            }

            if (targ is Plant)
            {
                return 5f;
            }

            if (targ.def.IsIngestible || targ.def.IsMeat)
            {
                return 3f;
            }

            if (targ.def.IsFilth)
            {
                return 4f;
            }

            return 3f;
        }

        // Token: 0x0600004C RID: 76 RVA: 0x000043B1 File Offset: 0x000025B1
        public void DoUSAcidGasPawn(Thing Gas, Thing targ)
        {
            if (PawnIsAlive(targ as Pawn))
            {
                DoAcidDamPawn(targ as Pawn);
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x000043CC File Offset: 0x000025CC
        public static void DoAcidDamPawn(Pawn pawn)
        {
            if (pawn == null || pawn.Map == null)
            {
                return;
            }

            SetUpBDVars(pawn, pawn, out var candidate, out var DamDef, out var dmg);
            if (candidate == null)
            {
                return;
            }

            var dinfo = new DamageInfo(DamDef, dmg, 0f, -1f, pawn, candidate);
            pawn.TakeDamage(dinfo);
            DoAcidMiniEffectMech(pawn);
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00004424 File Offset: 0x00002624
        public static void DoAcidMiniEffectMech(Pawn pawn)
        {
            if (!PawnIsAlive(pawn) || !pawn.RaceProps.IsMechanoid)
            {
                return;
            }

            FleckMaker.ThrowSmoke(pawn.Position.ToVector3(), pawn.Map, 1f);
            FleckMaker.ThrowMicroSparks(pawn.Position.ToVector3(), pawn.Map);
        }

        // Token: 0x0600004F RID: 79 RVA: 0x0000447D File Offset: 0x0000267D
        public static int Rnd100()
        {
            return Random.Range(1, 100);
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00004488 File Offset: 0x00002688
        public static void SetUpBDVars(Pawn Victim, Thing instigator, out BodyPartRecord candidate,
            out DamageDef DamDef, out float dmg)
        {
            DamDef = USAcid;
            dmg = Mathf.Lerp(2f, 5f, Rnd100() / 100f);
            if (Victim != null)
            {
                var bodyV = Victim.BodySize;
                if (bodyV > 0f)
                {
                    dmg = Math.Max(1f, dmg * bodyV);
                }

                if (!Victim.RaceProps.IsMechanoid)
                {
                    if (Victim.RaceProps.FleshType.defName == "Insectoid")
                    {
                        dmg = Math.Max(1f, dmg / 2.5f);
                    }
                    else if (Victim.RaceProps.Animal)
                    {
                        dmg = Math.Max(1f, dmg / 1.5f);
                    }
                    else
                    {
                        dmg = Math.Max(1f, dmg / 1.75f);
                    }
                }
                else
                {
                    dmg *= 1.25f;
                }
            }

            var toxRatio = Settings.USToxLevels / 100f;
            var sensitivity = 1f;
            if (Victim != null && !Victim.RaceProps.IsMechanoid)
            {
                sensitivity = Victim.GetStatValue(StatDefOf.ToxicSensitivity);
            }

            dmg *= toxRatio * sensitivity;
            candidate = null;
            var potentials = new List<BodyPartRecord>();
            if (Victim != null)
            {
                var raceProps = Victim.RaceProps;
                bool hasParts;
                if (raceProps == null)
                {
                    hasParts = false;
                }
                else
                {
                    var body = raceProps.body;
                    hasParts = body?.AllParts != null;
                }

                if (hasParts)
                {
                    potentials.AddRange(Victim.RaceProps.body.AllParts);
                }
            }

            if (potentials.Count > 0)
            {
                candidate = GetCandidate(potentials, Victim);
            }
        }

        // Token: 0x06000051 RID: 81 RVA: 0x000045F8 File Offset: 0x000027F8
        public static BodyPartRecord GetCandidate(List<BodyPartRecord> potentials, Pawn Victim)
        {
            var candidates = new List<BodyPartRecord>();
            foreach (var BPR in potentials)
            {
                if (BPR.IsCorePart)
                {
                    continue;
                }

                if (Victim.RaceProps.IsMechanoid)
                {
                    if (!BPR.def.defName.EndsWith("CentipedeBodyFirstRing"))
                    {
                        candidates.AddDistinct(BPR);
                    }
                }
                else if (BPR.depth != BodyPartDepth.Inside)
                {
                    candidates.AddDistinct(BPR);
                }
            }

            if (candidates.Count <= 0)
            {
                return null;
            }

            var candidate = candidates.RandomElement();
            if (candidate.IsCorePart)
            {
                candidate = null;
            }

            return candidate;
        }

        // Token: 0x06000052 RID: 82 RVA: 0x000046AC File Offset: 0x000028AC
        public void DoUSAcidGasToxic(Thing Gas, Thing targ)
        {
            if (targ is not Pawn victim || !victim.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
            {
                return;
            }

            var health = victim.health;
            Hediff hediff;
            if (health == null)
            {
                hediff = null;
            }
            else
            {
                var hediffSet = health.hediffSet;
                hediff = hediffSet?.GetFirstHediffOfDef(Globals.USAcidGasEffect);
            }

            var sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity);
            var addsev = Rand.Range(0.01f * sensitivity * toxicRatio, 0.1f * sensitivity * toxicRatio);
            if (Globals.USVictimImmuneTo(victim, Globals.USAcidGasEffect))
            {
                return;
            }

            if (hediff != null && addsev > 0f)
            {
                hediff.Severity += addsev;
                return;
            }

            var addhediff = HediffMaker.MakeHediff(Globals.USAcidGasEffect, victim);
            addhediff.Severity = addsev;
            victim.health.AddHediff(addhediff);
        }
    }
}