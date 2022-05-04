using RimWorld;
using Verse;

namespace USToxins;

public class USTanglerootGas : Gas
{
    public float toxicRatio = Settings.USToxLevels / 100f;

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
        if (destroyTick <= Find.TickManager.TicksGame)
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
        if (Thinglist.Count <= 0)
        {
            return;
        }

        for (var index = 0; index < Thinglist.Count; index++)
        {
            var thing = Thinglist[index];
            if (thing is Pawn pawn && !pawn.RaceProps.IsMechanoid &&
                pawn.RaceProps.FleshType.defName != "Insectoid" &&
                pawn.Position == TargetCell)
            {
                DoUSTangleKillGasToxic(this, pawn);
            }
        }
    }

    public void DoUSTangleKillGasToxic(Thing Gas, Thing targ)
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
            hediff = hediffSet?.GetFirstHediffOfDef(Globals.USTangleRootStrike);
        }

        var sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity);
        var addsev = Rand.Range(0.01f * sensitivity * toxicRatio, 0.05f * sensitivity * toxicRatio);
        if (Globals.USVictimImmuneTo(victim, Globals.USTangleRootStrike))
        {
            return;
        }

        if (hediff != null && addsev > 0f)
        {
            if (hediff.Severity + addsev < 1.8f)
            {
                hediff.Severity += addsev;
            }
        }
        else
        {
            var addhediff = HediffMaker.MakeHediff(Globals.USTangleRootStrike, victim);
            addhediff.Severity = addsev;
            victim.health.AddHediff(addhediff);
        }
    }
}