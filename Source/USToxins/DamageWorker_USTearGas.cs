using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x0200000C RID: 12
    public class DamageWorker_USTearGas : DamageWorker
    {
        // Token: 0x04000003 RID: 3
        public float toxicRatio = Settings.USToxLevels / 100f;

        // Token: 0x0600001D RID: 29 RVA: 0x00002B58 File Offset: 0x00000D58
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

        // Token: 0x0600001E RID: 30 RVA: 0x00002BE8 File Offset: 0x00000DE8
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