using RimWorld;
using Verse;

namespace USToxins;

public class USFilthKiller : Filth
{
    private int USspawnTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref USspawnTick, "USspawnTick");
    }

    public override void Tick()
    {
        USspawnTick++;
        if (USspawnTick >= 60000)
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
        var Filthlist = TargetCell.GetThingList(TargetMap);
        if (Filthlist.Count <= 0)
        {
            return;
        }

        for (var index = 0; index < Filthlist.Count; index++)
        {
            var thing = Filthlist[index];
            if (thing is not Filth || thing.Position != TargetCell || thing.def.defName == "Filth_USFilthKillFoam")
            {
                continue;
            }

            var CleaningAmount = thing.def.filth.cleaningWorkToReduceThickness * 60f;
            var toxRatio = Settings.USToxLevels / 100f;
            if (toxRatio > 0f)
            {
                CleaningAmount /= toxRatio;
            }

            if (Find.TickManager.TicksGame - USspawnTick > CleaningAmount)
            {
                DoUSToxicDamage(thing);
            }
        }
    }

    private void DoUSToxicDamage(Thing targ)
    {
        if (targ is Filth filth)
        {
            filth.ThinFilth();
        }
    }
}