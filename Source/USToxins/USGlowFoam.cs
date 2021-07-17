using RimWorld;
using Verse;

namespace USToxins
{
    // Token: 0x0200001C RID: 28
    public class USGlowFoam : Filth
    {
        // Token: 0x04000043 RID: 67
        private int USGlowTick;

        // Token: 0x04000042 RID: 66
        private int USspawnTick;

        // Token: 0x06000062 RID: 98 RVA: 0x00004B41 File Offset: 0x00002D41
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref USspawnTick, "USspawnTick");
            Scribe_Values.Look(ref USGlowTick, "USGlowTick");
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00004B70 File Offset: 0x00002D70
        public override void Tick()
        {
            USspawnTick++;
            if (USspawnTick >= 20000)
            {
                Destroy();
                return;
            }

            USGlowTick--;
            if (USGlowTick > 0)
            {
                return;
            }

            FleckMaker.ThrowHeatGlow(Position, Map, 0.8f);
            USGlowTick = 90;
        }
    }
}