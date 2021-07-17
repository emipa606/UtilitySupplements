using System;
using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200002E RID: 46
    public class USPlantKiller : Filth
    {
        // Token: 0x04000050 RID: 80
        private int USspawnTick;

        // Token: 0x060000A5 RID: 165 RVA: 0x000065AD File Offset: 0x000047AD
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x060000A6 RID: 166 RVA: 0x000065C8 File Offset: 0x000047C8
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

            foreach (var thing in Plantlist)
            {
                if (thing is Plant plant && plant.Position == TargetCell &&
                    plant.def.plant.purpose != PlantPurpose.Health)
                {
                    DoUSToxicDamage(plant);
                }
            }
        }

        // Token: 0x060000A7 RID: 167 RVA: 0x00006684 File Offset: 0x00004884
        private void DoUSToxicDamage(Thing targ)
        {
            if (targ is not Plant plant || plant.def.defName == "Plant_USStinkroot")
            {
                return;
            }

            var Dmg = Math.Max(1f, 2f * (Settings.USToxLevels / 100f));
            plant.TakeDamage(new DamageInfo(Globals.USPlantToxin, Dmg, 0f, -1f, this));
        }
    }
}