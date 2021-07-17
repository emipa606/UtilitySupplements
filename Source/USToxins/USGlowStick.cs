using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200001D RID: 29
    public class USGlowStick : Filth
    {
        // Token: 0x04000045 RID: 69
        private int USGlowTick;

        // Token: 0x04000044 RID: 68
        private int USspawnTick;

        // Token: 0x06000065 RID: 101 RVA: 0x00004BDD File Offset: 0x00002DDD
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
            Scribe_Values.Look(ref USGlowTick, "USGlowTick");
        }

        // Token: 0x06000066 RID: 102 RVA: 0x00004C0C File Offset: 0x00002E0C
        public override void Tick()
        {
            USspawnTick++;
            if (USspawnTick > 30000)
            {
                Destroy();
                return;
            }

            USGlowTick--;
            if (USGlowTick > 0)
            {
                return;
            }

            FleckMaker.ThrowHeatGlow(Position, Map, 2f);
            USGlowTick = 90;
        }
    }
}