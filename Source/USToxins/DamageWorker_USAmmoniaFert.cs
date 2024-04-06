using System.Collections.Generic;
using RimWorld;
using Verse;

namespace USToxins;

public class DamageWorker_USAmmoniaFert : DamageWorker
{
    public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
    {
        FleckMaker.ThrowSmoke(explosion.Position.ToVector3(), explosion.Map, 1f);
        ExplosionVisualEffectCenter(explosion);
    }
}