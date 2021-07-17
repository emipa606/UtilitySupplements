using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USToxins
{
    // Token: 0x0200002B RID: 43
    public class DamageWorker_USTreeKill : DamageWorker
    {
        // Token: 0x0600009B RID: 155 RVA: 0x0000619C File Offset: 0x0000439C
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

        // Token: 0x0600009C RID: 156 RVA: 0x0000622C File Offset: 0x0000442C
        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            var damageResult = new DamageResult();
            if (victim.def.category != ThingCategory.Plant || !victim.def.plant.IsTree || !victim.def.useHitPoints ||
                !dinfo.Def.harmsHealth)
            {
                return damageResult;
            }

            var num = dinfo.Amount;
            num *= dinfo.Def.plantDamageFactor;
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