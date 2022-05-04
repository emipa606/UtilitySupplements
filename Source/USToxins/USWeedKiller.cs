using RimWorld;
using Verse;

namespace USToxins;

public class USWeedKiller : Filth
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
        var Plantlist = TargetCell.GetThingList(TargetMap);
        if (Plantlist.Count <= 0)
        {
            return;
        }

        for (var index = 0; index < Plantlist.Count; index++)
        {
            var thing = Plantlist[index];
            if (thing is Plant plant && plant.Position == TargetCell &&
                plant.def.plant.purpose == PlantPurpose.Misc &&
                !plant.def.plant.IsTree)
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

        var Dmg = 2f * (Settings.USToxLevels / 100f);
        if (Dmg < 1f)
        {
            Dmg = 1f;
        }

        plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this));
    }
}