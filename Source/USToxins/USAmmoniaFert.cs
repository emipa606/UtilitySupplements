using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000019 RID: 25
    public class USAmmoniaFert : Filth
    {
        // Token: 0x0400003E RID: 62
        private int AYspawnTick;

        // Token: 0x06000055 RID: 85 RVA: 0x000047CA File Offset: 0x000029CA
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AYspawnTick, "AYspawnTick");
        }

        // Token: 0x06000056 RID: 86 RVA: 0x000047E4 File Offset: 0x000029E4
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

            foreach (var thing in Thinglist)
            {
                if (thing is Plant && thing.Position == TargetCell)
                {
                    DoAYPlantBenefit(thing, thickness);
                }
            }
        }

        // Token: 0x06000057 RID: 87 RVA: 0x00004894 File Offset: 0x00002A94
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

        // Token: 0x06000058 RID: 88 RVA: 0x000048F4 File Offset: 0x00002AF4
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
}