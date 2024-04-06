using RimWorld;
using Verse;

namespace USToxins;

public class USGlowFoam : Filth
{
    private int USGlowTick;

    private int USspawnTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref USspawnTick, "USspawnTick");
        Scribe_Values.Look(ref USGlowTick, "USGlowTick");
    }

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