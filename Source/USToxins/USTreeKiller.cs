using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200002F RID: 47
    public class USTreeKiller : Filth
    {
        // Token: 0x04000051 RID: 81
        private int USspawnTick;

        // Token: 0x060000A9 RID: 169 RVA: 0x000066F7 File Offset: 0x000048F7
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x060000AA RID: 170 RVA: 0x00006714 File Offset: 0x00004914
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
                    (plant.def.plant.IsTree ||
                     plant.def.plant.purpose == PlantPurpose.Misc))
                {
                    DoUSToxicDamage(plant);
                }
            }
        }

        // Token: 0x060000AB RID: 171 RVA: 0x000067F0 File Offset: 0x000049F0
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
}