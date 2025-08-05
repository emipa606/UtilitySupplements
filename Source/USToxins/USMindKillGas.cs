using RimWorld;
using Verse;

namespace USToxins;

public class USMindKillGas : Gas
{
    private readonly float toxicRatio = Settings.USToxLevels / 100f;

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

    protected override void Tick()
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

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < Thinglist.Count; index++)
        {
            var thing = Thinglist[index];
            if (thing is Pawn && thing.Position == TargetCell)
            {
                DoUSMindKillGasToxic(thing);
            }

            if (thing is Plant plant && plant.def.plant.purpose != PlantPurpose.Health &&
                plant.def.defName != "Plant_USStinkroot")
            {
                DoUSToxicDamage(plant);
            }
        }
    }

    private void DoUSMindKillGasToxic(Thing targ)
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
            hediff = hediffSet?.GetFirstHediffOfDef(Globals.USStinkRootGas);
        }

        var sensitivity = victim.GetStatValue(StatDefOf.ToxicResistance);

        var addsev = 0.25f * toxicRatio;
        if (sensitivity != 0)
        {
            addsev /= sensitivity;
        }

        if (Globals.USVictimImmuneTo(victim, Globals.USStinkRootGas))
        {
            return;
        }

        if (hediff != null && addsev > 0f)
        {
            hediff.Severity += addsev;
            if (hediff.Severity >= 2.5f)
            {
                DoUSToxicMental(victim, hediff);
            }
        }
        else
        {
            var addhediff = HediffMaker.MakeHediff(Globals.USStinkRootGas, victim);
            addhediff.Severity = addsev;
            victim.health.AddHediff(addhediff);
        }
    }

    private void DoUSToxicDamage(Thing targ)
    {
        if (targ is not Plant plant || plant.def.defName == "Plant_USStinkroot")
        {
            return;
        }

        var Dmg = 5f * toxicRatio;
        if (Dmg < 1f)
        {
            Dmg = 1f;
        }

        plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this));
    }

    private static void DoUSToxicMental(Pawn victim, Hediff hediff)
    {
        if (victim.RaceProps.IsMechanoid || victim.RaceProps.FleshType.defName == "Insectoid" ||
            !(hediff.def.maxSeverity > 0f) ||
            !(Rand.Range(0f + (0.2f * (hediff.Severity / hediff.def.maxSeverity)), 1f) >= 0.95f) ||
            victim.mindState.mentalStateHandler.InMentalState)
        {
            return;
        }

        var whichBreak = Rand.Range(0f, 1f);
        switch (whichBreak)
        {
            case > 0.75f:
                victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);
                return;
            case >= 0.45f and <= 0.75f:
                victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Psychotic);
                return;
            case < 0.45f:
                victim.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
                break;
        }
    }
}