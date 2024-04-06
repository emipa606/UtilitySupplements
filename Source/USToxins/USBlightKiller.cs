using RimWorld;
using Verse;

namespace USToxins;

public class USBlightKiller : Filth
{
    public readonly float toxicRatio = Settings.USToxLevels / 100f;

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
        var Blightlist = TargetCell.GetThingList(TargetMap);
        if (Blightlist.Count <= 0)
        {
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < Blightlist.Count; index++)
        {
            var thing = Blightlist[index];
            if (thing is Blight && thing.Position == TargetCell)
            {
                DoUSToxicDamage(thing);
            }
        }
    }

    private void DoUSToxicDamage(Thing targ)
    {
        if (targ is not Blight blight)
        {
            return;
        }

        var Dmg = 0.1f * toxicRatio;
        if (Dmg < 1f)
        {
            Dmg = 1f;
        }

        blight.Severity -= Dmg;
        if (blight.Severity <= 0f)
        {
            blight.Destroy();
        }
    }
}