using RimWorld;
using Verse;

namespace USToxins;

public class USTreeKiller : Filth
{
    private int USspawnTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref USspawnTick, "USspawnTick");
    }

    protected override void Tick()
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
        var Plantlist = TargetCell.GetThingList(TargetMap);
        if (Plantlist.Count <= 0)
        {
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < Plantlist.Count; index++)
        {
            var thing = Plantlist[index];
            if (thing is Plant plant && plant.Position == TargetCell &&
                (plant.def.plant.IsTree ||
                 plant.def.plant.purpose == PlantPurpose.Misc))
            {
                DoUSToxicDamage(plant);
            }
        }
    }

    private void DoUSToxicDamage(Thing targ)
    {
        if (targ is not Plant plant || plant.def.defName == "Plant_USStinkroot")
        {
            return;
        }

        var Dmg = 4f * (Settings.USToxLevels / 100f);
        if (Dmg < 2f)
        {
            Dmg = 2f;
        }

        plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this));
    }
}