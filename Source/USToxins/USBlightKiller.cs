using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200001B RID: 27
    public class USBlightKiller : Filth
    {
        // Token: 0x04000041 RID: 65
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x04000040 RID: 64
        private int USspawnTick;

        // Token: 0x0600005E RID: 94 RVA: 0x00004A18 File Offset: 0x00002C18
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x0600005F RID: 95 RVA: 0x00004A34 File Offset: 0x00002C34
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
            var Blightlist = TargetCell.GetThingList(TargetMap);
            if (Blightlist.Count <= 0)
            {
                return;
            }

            foreach (var thing in Blightlist)
            {
                if (thing is Blight && thing.Position == TargetCell)
                {
                    DoUSToxicDamage(thing);
                }
            }
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00004AD4 File Offset: 0x00002CD4
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
}