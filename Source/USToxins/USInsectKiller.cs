using RimWorld;
using Verse;

namespace USToxins;

public class USInsectKiller : Filth
{
    public readonly float toxRatio = Settings.USToxLevels / 100f;

    private int USspawnTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref USspawnTick, "USspawnTick");
    }

    public override void Tick()
    {
        USspawnTick++;
        if (USspawnTick >= GenDate.TicksPerDay)
        {
            Destroy();
            return;
        }

        if (Find.TickManager.TicksGame % 120 != 0)
        {
            return;
        }

        var TargetMap = Map;
        var TargetCell = Position;
        var Pawnlist = TargetCell.GetThingList(TargetMap);
        if (Pawnlist.Count <= 0)
        {
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < Pawnlist.Count; index++)
        {
            var thing = Pawnlist[index];
            if (thing is not Pawn pawn || pawn.Position != TargetCell ||
                pawn.RaceProps.FleshType.defName != "Insectoid")
            {
                continue;
            }

            DoUSToxicDamage(pawn);
            ApplyUSToxicHediff(pawn);
        }
    }

    private void DoUSToxicDamage(Pawn I)
    {
        if (I == null)
        {
            return;
        }

        var Dmg = 3f * toxRatio;
        if (Dmg < 1f)
        {
            Dmg = 1f;
        }

        I.TakeDamage(new DamageInfo(DamageDefOf.Flame, Dmg, 0f, -1f, this));
    }

    private void ApplyUSToxicHediff(Pawn I)
    {
        if (I == null)
        {
            return;
        }

        var health = I.health;
        Hediff hediff;
        if (health == null)
        {
            hediff = null;
        }
        else
        {
            var hediffSet = health.hediffSet;
            hediff = hediffSet?.GetFirstHediffOfDef(HediffDefOf.Heatstroke);
        }

        var addSev = 0.02f * toxRatio;
        if (addSev < 0.01f)
        {
            addSev = 0.01f;
        }

        if (hediff != null)
        {
            hediff.Severity += addSev;
            return;
        }

        var addhediff = HediffMaker.MakeHediff(HediffDefOf.Heatstroke, I);
        addhediff.Severity = addSev;
        I.health.AddHediff(addhediff);
    }
}