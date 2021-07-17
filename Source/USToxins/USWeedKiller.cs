using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x02000030 RID: 48
    public class USWeedKiller : Filth
    {
        // Token: 0x04000052 RID: 82
        private int USspawnTick;

        // Token: 0x060000AD RID: 173 RVA: 0x00006867 File Offset: 0x00004A67
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x060000AE RID: 174 RVA: 0x00006884 File Offset: 0x00004A84
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
                    plant.def.plant.purpose == PlantPurpose.Misc &&
                    !plant.def.plant.IsTree)
                {
                    DoUSToxicDamage(plant);
                }
            }
        }

        // Token: 0x060000AF RID: 175 RVA: 0x00006960 File Offset: 0x00004B60
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
}