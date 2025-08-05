using System;
using RimWorld;
using Verse;

namespace USToxins;

public class USAntifreeze : Filth
{
    private int USspawnTick;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref USspawnTick, "USspawnTick");
    }

    protected override void Tick()
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

    private static float GetRndMelt()
    {
        return Rand.Range(0.1f, 0.2f);
    }
}