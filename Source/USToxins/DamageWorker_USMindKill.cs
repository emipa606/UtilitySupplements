using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x0200000B RID: 11
    public class DamageWorker_USMindKill : DamageWorker
    {
        // Token: 0x04000002 RID: 2
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x0600001A RID: 26 RVA: 0x000029D8 File Offset: 0x00000BD8
        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map,
                    def.explosionHeatEnergyPerCell * cellsToAffect.Count);
            }

            FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
            FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
            ExplosionVisualEffectCenter(explosion);
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002A68 File Offset: 0x00000C68
        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var damageResult = new DamageResult();
            if (victim.def.category != ThingCategory.Pawn ||
                !((Pawn) victim).health.capacities.CapableOf(PawnCapacityDefOf.Breathing) ||
                !victim.def.useHitPoints || !dinfo.Def.harmsHealth)
            {
                return damageResult;
            }

            var num = dinfo.Amount;
            if (num > 0f)
            {
                var sensitivity = victim.GetStatValue(StatDefOf.ToxicSensitivity);
                num = num * sensitivity * toxicRatio;
            }

            damageResult.totalDamageDealt = Mathf.Min(victim.HitPoints, GenMath.RoundRandom(num));
            victim.HitPoints -= Mathf.RoundToInt(damageResult.totalDamageDealt);
            if (victim.HitPoints > 0)
            {
                return damageResult;
            }

            victim.HitPoints = 0;
            victim.Kill(dinfo);

            return damageResult;
        }
    }
}