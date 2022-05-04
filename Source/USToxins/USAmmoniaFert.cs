using RimWorld;
using Verse;

namespace USToxins;

public class USAmmoniaFert : Filth
{
    private int AYspawnTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref AYspawnTick, "AYspawnTick");
    }

    public override void Tick()
    {
        AYspawnTick++;
        var removeDelay = 300000;
        if (AYspawnTick >= removeDelay)
        {
            Destroy();
            return;
        }

        if (Find.TickManager.TicksGame % 295 != 0)
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
            if (thing is Plant && thing.Position == TargetCell)
            {
                DoAYPlantBenefit(thing, thickness);
            }
        }
    }

    private void DoAYPlantBenefit(Thing targ, int thick)
    {
        if (targ is not Plant plant || !(plant.Growth < 1f))
        {
            return;
        }

        plant.Growth += AYGrowthPerTick(plant) * (1f - (thick / 20f));
        if (plant.Growth > 1f)
        {
            plant.Growth = 1f;
        }
    }

    private float AYGrowthPerTick(Plant plant)
    {
        if (plant.LifeStage != PlantLifeStage.Growing || GenLocalDate.DayPercent(plant) < 0.25f ||
            GenLocalDate.DayPercent(plant) > 0.8f)
        {
            return 0f;
        }

        return 1f / (60000f * plant.def.plant.growDays) * plant.GrowthRate * 500f;
    }
}