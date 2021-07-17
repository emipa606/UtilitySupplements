using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200002D RID: 45
    public class USFilthKiller : Filth
    {
        // Token: 0x0400004F RID: 79
        private int USspawnTick;

        // Token: 0x060000A1 RID: 161 RVA: 0x00006450 File Offset: 0x00004650
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x060000A2 RID: 162 RVA: 0x0000646C File Offset: 0x0000466C
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

            foreach (var thing in Filthlist)
            {
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

        // Token: 0x060000A3 RID: 163 RVA: 0x00006588 File Offset: 0x00004788
        private void DoUSToxicDamage(Thing targ)
        {
            if (targ is Filth filth)
            {
                filth.ThinFilth();
            }
        }
    }
}