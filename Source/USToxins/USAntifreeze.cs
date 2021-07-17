using System;
using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200001A RID: 26
    public class USAntifreeze : Filth
    {
        // Token: 0x0400003F RID: 63
        private int USspawnTick;

        // Token: 0x0600005A RID: 90 RVA: 0x0000495B File Offset: 0x00002B5B
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00004978 File Offset: 0x00002B78
        public override void Tick()
        {
            USspawnTick++;
            var removeDelay = 120000;
            if (USspawnTick >= removeDelay)
            {
                Destroy();
                return;
            }

            if (Find.TickManager.TicksGame % 2497 != 0)
            {
                return;
            }

            var TargetMap = Map;
            var TargetCell = Position;
            var SnowDepth = TargetMap.snowGrid.GetDepth(TargetCell);
            if (!(SnowDepth > 0f))
            {
                return;
            }

            SnowDepth -= Math.Max(0f, GetRndMelt());
            TargetMap.snowGrid.SetDepth(TargetCell, SnowDepth);
        }

        // Token: 0x0600005C RID: 92 RVA: 0x000049FF File Offset: 0x00002BFF
        public float GetRndMelt()
        {
            return Rand.Range(0.1f, 0.2f);
        }
    }
}