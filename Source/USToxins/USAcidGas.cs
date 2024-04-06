using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace USToxins;

public class USAcidGas : Gas
{
    public static readonly DamageDef USAcid = DefDatabase<DamageDef>.GetNamed("Dam_USAcidGas");

    public readonly float toxicRatio = Settings.USToxLevels / 100f;

    public int amountAcidDam = 1000;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, true);
        destroyTick = Find.TickManager.TicksGame + def.gas.expireSeconds.RandomInRange.SecondsToTicks();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref destroyTick, "destroyTick");
    }

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
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < Thinglist.Count; index++)
            {
                var thing = Thinglist[index];
                if (thing is Gas || thing.Position != TargetCell)
                {
                    continue;
                }

                if (thing.def.useHitPoints && thing is not Pawn ||
                    thing.def.useHitPoints && thing.def.IsCorpse)
                {
                    DoUSAcidGasDegrade(this, thing, out var DMG);
                    amountAcidDam -= DMG;
                    continue;
                }

                if (thing is not Pawn pawn || pawn.def.IsCorpse)
                {
                    continue;
                }

                DoUSAcidGasPawn(this, pawn);
                if (PawnIsAlive(pawn) &&
                    !pawn.RaceProps.IsMechanoid &&
                    pawn.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
                {
                    DoUSAcidGasToxic(this, pawn);
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

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < cells.Count; index++)
        {
            var cell = cells[index];
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

    public static bool PawnIsAlive(Pawn pawn)
    {
        return pawn is { Spawned: true, Dead: false } && !pawn.def.IsCorpse;
    }

    public void DoUSAcidGasDegrade(Thing Gas, Thing targ, out int DMG)
    {
        var baseDMG = Math.Max(1f, Math.Min(10f, GetBaseDMG(targ)));
        DMG = (int)baseDMG;
        targ.HitPoints -= DMG;
        if (targ.HitPoints <= 0 && !targ.Destroyed)
        {
            targ.Destroy();
        }
    }

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

        return targ.def.IsFilth ? 4f : 3f;
    }

    public void DoUSAcidGasPawn(Thing Gas, Thing targ)
    {
        if (PawnIsAlive(targ as Pawn))
        {
            DoAcidDamPawn(targ as Pawn);
        }
    }

    public static void DoAcidDamPawn(Pawn pawn)
    {
        if (pawn?.Map == null)
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

    public static void DoAcidMiniEffectMech(Pawn pawn)
    {
        if (!PawnIsAlive(pawn) || !pawn.RaceProps.IsMechanoid)
        {
            return;
        }

        FleckMaker.ThrowSmoke(pawn.Position.ToVector3(), pawn.Map, 1f);
        FleckMaker.ThrowMicroSparks(pawn.Position.ToVector3(), pawn.Map);
    }

    public static int Rnd100()
    {
        return Random.Range(1, 100);
    }

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
            sensitivity = Victim.GetStatValue(StatDefOf.ToxicEnvironmentResistance);
        }

        dmg *= toxRatio;
        if (sensitivity != 0)
        {
            dmg /= sensitivity;
        }

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

        var sensitivity = victim.GetStatValue(StatDefOf.ToxicEnvironmentResistance);
        var num = toxicRatio;
        if (sensitivity != 0)
        {
            num /= sensitivity;
        }

        var addsev = Rand.Range(0.01f * num, 0.1f * num);
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